using Microsoft.AspNetCore.Mvc;
using Net.payOS;
using Net.payOS.Types;
using Project.Core.Entities.Business.DTOs;
using Project.Core.Entities.Business.DTOs.PaymentDTOs;
using Project.Core.Entities.Business.DTOs.PaymentDTOs.MyApi.Models;
using Project.Core.Entities.General;
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
        private readonly ISubscriptionPlanRepository _subscriptionPlanRepository;
        private readonly IUserSubscriptionRepository _userSubscriptionRepository;

        public PaymentController(PayOS payOS, ApplicationDbContext context) {
            _payOS = payOS;
            _context = context;
            _paymentRepository = new PaymentRepository(context);
            _subscriptionPlanRepository = new SubscriptionPlanRepository(context);
            _userSubscriptionRepository = new UserSubscriptionRepository(context);
        }

        [HttpGet("GetAllPlan")]
        public async Task<IActionResult> GetAllPlan() {
            try {
                var plans = await _subscriptionPlanRepository.GetAll();
                return Ok(new APIResponse {
                    result = plans,
                });
            }
            catch (Exception ex) {
                return BadRequest(new APIResponse {
                    errorMessages = new List<string> { ex.Message }
                });
            }
        }

        [HttpPost("CreatePaymentLink")]
        public async Task<IActionResult> CreatePaymentLink([FromBody] CreatePaymentRequest request) {
            try {
                var productName = request.productName ?? "anonymous";
                var orderCode = int.Parse(DateTimeOffset.Now.ToString("ffffff"));

                var items = new List<ItemData>
                {
            new ItemData(productName, request.durationMonth, request.price)
        };

                var paymentData = new PaymentData(
                    orderCode,
                    request.price * request.durationMonth,
                    request.description,
                    items,
                    request.cancelUrl,
                    request.returnUrl
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
                            EndDate = DateTime.UtcNow.AddMonths(request.durationMonth),
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
                    Amount = request.price * request.durationMonth,
                    TransactionId = createPayment.paymentLinkId,
                    OrderCode = (int)createPayment.orderCode,
                    Status = "PENDING",
                    CreatedAt = DateTime.UtcNow,
                    PaymentMethod = request.paymentMethod,
                    Currency = createPayment.currency,
                    UserId = request.UserId,
                    SubscriptionId = userSubscription.Id
                };

                await _paymentRepository.Create(payment);

                var response = new CreatePaymentLinkResponse {
                    CheckoutUrl = createPayment.checkoutUrl,
                    Message = "Create payment link successful",
                };

                return Ok(new APIResponse { result = response });
            }
            catch (Exception ex) {
                return BadRequest(new APIResponse {
                    errorMessages = new List<string> { ex.Message }
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

            PaymentLinkInformation paymentLinkInformation;
            try {
                paymentLinkInformation = await _payOS.getPaymentLinkInformation(returnUrlQuery.orderCode);
            }
            catch (Exception) {
                return BadRequest(new APIResponse {
                    errorMessages = new List<string> { "Error retrieving payment link information." }
                });
            }

            var newStatus = "null";
            var resultMessage = "null";

            if (returnUrlQuery.status.Equals("PAID", StringComparison.OrdinalIgnoreCase) &&
                paymentLinkInformation.status.Equals("PAID", StringComparison.OrdinalIgnoreCase) &&
                returnUrlQuery.code == "00") {
                newStatus = "PAID";
                resultMessage = "Payment successful";
            }
            else if (returnUrlQuery.status.Equals("CANCELLED", StringComparison.OrdinalIgnoreCase) ||
                       returnUrlQuery.cancel) {
                newStatus = "CANCELLED";
                resultMessage = "Payment cancelled";
            }
            else if (returnUrlQuery.status.Equals("PENDING", StringComparison.OrdinalIgnoreCase) ||
                       returnUrlQuery.status.Equals("PROCESSING", StringComparison.OrdinalIgnoreCase)) {
                newStatus = returnUrlQuery.status.ToUpper();
                resultMessage = "Payment is pending";
            }
            else {
                return BadRequest(new APIResponse {
                    errorMessages = new List<string> { "Payment failed" }
                });
            }

            var payment = await _paymentRepository.GetPaymentByOrderCodeAsync(returnUrlQuery.orderCode);
            if (payment == null) {
                return NotFound(new APIResponse {
                    errorMessages = new List<string> { "Payment not found" }
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
                userSubscription.EndDate = userSubscription.EndDate.AddMonths(returnUrlQuery.period);
            }
            else {
                userSubscription.StartDate = DateTime.UtcNow;
                userSubscription.EndDate = userSubscription.StartDate.AddMonths(returnUrlQuery.period);
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
    }
}
