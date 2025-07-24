using Microsoft.AspNetCore.Mvc;
using Net.payOS;
using Net.payOS.Types;
using Project.Core.Entities.Business.DTOs;
using Project.Core.Entities.Business.DTOs.PaymentDTOs;
using Project.Core.Entities.Business.DTOs.PaymentDTOs.MyApi.Models;
using Project.Core.Entities.General;
using Project.Core.Interfaces.IMapper;
using Project.Core.Interfaces.IRepositories;
using Project.Infrastructure.Data;
using Project.Infrastructure.Repositories;

namespace Project.API.Controllers {
    [Route("api/Payment")]
    [ApiController]
    public class PaymentController : ControllerBase {
        private readonly PayOS _payOS;
        private readonly ApplicationDbContext _context;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IProfileRepository _profileRepository;
        private readonly ISubscriptionPlanRepository _subscriptionPlanRepository;
        private readonly IUserSubscriptionRepository _userSubscriptionRepository;
        private readonly IBaseMapper<Payment, HistoryPaymentDTO> _mapper;


        public PaymentController(PayOS payOS, ApplicationDbContext context, IBaseMapper<Payment, HistoryPaymentDTO> mapper) {
            _payOS = payOS;
            _context = context;
            _paymentRepository = new PaymentRepository(context);
            _subscriptionPlanRepository = new SubscriptionPlanRepository(context);
            _userSubscriptionRepository = new UserSubscriptionRepository(context);
            _profileRepository = new ProfileRepository(context);
            _mapper = mapper;
        }

        [HttpGet("GetAllPlan")]
        public async Task<IActionResult> GetAllPlan() {
            try {
                var plans = await _subscriptionPlanRepository.GetAll();
                return Ok(new APIResponse {
                    result = plans,
                });
            }
            catch (Exception) {
                return BadRequest(new APIResponse {
                    errorMessages = new List<string> { "failGetplan" }
                });
            }
        }

        [HttpPost("CreatePaymentLink")]
        public async Task<IActionResult> CreatePaymentLink([FromBody] CreatePaymentRequest request) {
            try {
                var productName = request.ProductName ?? "Subscription Plan";
                var orderCode = int.Parse(DateTimeOffset.Now.ToString("ffffff"));

                var items = new List<ItemData>
                {
            new ItemData(productName, request.DurationMonth, request.Price)
        };

                var paymentData = new PaymentData(
                    orderCode,
                    request.Price * request.DurationMonth,
                    request.Description,
                    items,
                    request.CancelUrl,
                    request.ReturnUrl
                );

                UserSubscription userSubscription = new UserSubscription();
                try {
                    userSubscription = await _userSubscriptionRepository.GetUserSubscriptionByBothIdAsync(request.UserId, request.SubscriptionId);

                }
                catch {
                    if (userSubscription.Id.Equals(Guid.Empty)) {
                        userSubscription = new UserSubscription {
                            Id = Guid.NewGuid(),
                            PlanId = request.SubscriptionId,
                            UserId = request.UserId,
                            CreatedAt = DateTime.UtcNow,
                            StartDate = DateTime.UtcNow,
                            EndDate = DateTime.UtcNow.AddMonths(request.DurationMonth),
                            Status = "UNACTIVE"
                        };
                    }

                    try {
                        await _userSubscriptionRepository.Create(userSubscription);
                    }
                    catch (Exception) {
                        return BadRequest(new APIResponse {
                            errorMessages = new List<string> { "Error Create User Subscription" }
                        });
                    }
                }

                var createPayment = await _payOS.createPaymentLink(paymentData);

                var payment = new Payment {
                    Id = Guid.NewGuid(),
                    Amount = request.Price * request.DurationMonth,
                    TransactionId = createPayment.paymentLinkId,
                    OrderCode = (int)createPayment.orderCode,
                    Status = "PENDING",
                    CreatedAt = DateTime.UtcNow,
                    PaymentMethod = request.PaymentMethod,
                    Currency = createPayment.currency,
                    UserId = request.UserId,
                    SubscriptionId = userSubscription.Id
                };

                var user = await _context.Profiles.FindAsync(request.UserId);
                if (user == null) {
                    return NotFound(new APIResponse {
                        errorMessages = new List<string> { "User not found" }
                    });
                }
                user.VipUser = true;

                await _profileRepository.Update(user);
                await _paymentRepository.Create(payment);

                var response = new CreatePaymentLinkResponse {
                    CheckoutUrl = createPayment.checkoutUrl,
                    Message = "Create payment link successful",
                };

                return Ok(new APIResponse { result = response });
            }
            catch (Exception) {
                return BadRequest(new APIResponse {
                    errorMessages = new List<string> { "errorCreatePaymentLink" }
                });
            }
        }


        [HttpPost("UpdatePlan")]
        public async Task<IActionResult> UpdatePlan([FromBody] ReturnUrlQuery returnUrlQuery) {
            if (returnUrlQuery == null) {
                return BadRequest(new APIResponse {
                    errorMessages = new List<string> { "Invalid request" }
                });
            }
            var payment = await _paymentRepository.GetPaymentByOrderCodeAsync(returnUrlQuery.OrderCode);
            if (payment == null) {
                return NotFound(new APIResponse {
                    errorMessages = new List<string> { "Payment not found" }
                });
            }

            PaymentLinkInformation paymentLinkInformation;
            try {
                paymentLinkInformation = await _payOS.getPaymentLinkInformation(returnUrlQuery.OrderCode);
            }
            catch (Exception) {
                return BadRequest(new APIResponse {
                    errorMessages = new List<string> { "errorRetrievingLinkInformation" }
                });
            }

            var newStatus = "null";
            var resultMessage = "null";

            if (returnUrlQuery.Status.Equals("PAID", StringComparison.OrdinalIgnoreCase) &&
                paymentLinkInformation.status.Equals("PAID", StringComparison.OrdinalIgnoreCase) &&
                returnUrlQuery.Code == "00") {
                newStatus = "PAID";
                resultMessage = "Payment successful";

            }
            else if (returnUrlQuery.Status.Equals("CANCELLED", StringComparison.OrdinalIgnoreCase) ||
                               returnUrlQuery.Cancel) {
                newStatus = "CANCELLED";
                resultMessage = "Payment cancelled";

            }
            else {
                return BadRequest(new APIResponse {
                    errorMessages = new List<string> { "Payment failed" }
                });
            }

            try {
                payment.Status = newStatus;
                await _paymentRepository.Update(payment);
            }
            catch (Exception ex) {
                return BadRequest(new APIResponse {
                    errorMessages = new List<string> { $"Error updating payment: {ex.Message}" }
                });
            }

            var userSubscription = await _userSubscriptionRepository.GetById(payment.SubscriptionId);
            if (userSubscription == null) {
                return NotFound(new APIResponse {
                    errorMessages = new List<string> { "User subscription not found" }
                });
            }

            if (newStatus.Equals("PAID", StringComparison.OrdinalIgnoreCase) && DateTime.UtcNow >= userSubscription.StartDate && DateTime.UtcNow <= userSubscription.EndDate) {
                userSubscription.EndDate = userSubscription.EndDate.AddMonths(returnUrlQuery.Period);
            }
            else {
                userSubscription.StartDate = DateTime.UtcNow;
                userSubscription.EndDate = userSubscription.StartDate.AddMonths(returnUrlQuery.Period);
            }

            try {
                userSubscription.Status = newStatus;
                await _userSubscriptionRepository.Update(userSubscription);
            }
            catch (Exception ex) {
                return BadRequest(new APIResponse {
                    errorMessages = new List<string> { $"Error updating user subscription: {ex.Message}" }
                });
            }

            return Ok(new APIResponse {
                result = resultMessage
            });
        }

        [HttpGet("GetAllPaymentInformation")]
        public async Task<IActionResult> GetAllPaymentInformation(Guid userId) {
            try {
                List<Payment> paymentInfos = await _paymentRepository.GetListPaymentByUserIdAsync(userId);
                List<HistoryPaymentDTO> historyPaymentInfos = _mapper.MapList(paymentInfos).ToList();

                return Ok(new APIResponse { result = historyPaymentInfos });
            }
            catch (Exception) {

                return BadRequest(new APIResponse {
                    errorMessages = new List<string> { "errorRetrievingLinkInformation" }
                });
            }
        }


    }
}
