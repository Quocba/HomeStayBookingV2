using System.Net;
using BusinessObject.DTO;
using BusinessObject.Entities;
using BusinessObject.Interfaces;
using BusinessObject.Shares;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VoucherController(IRepository<Voucher> _voucherRepository,
                                   IRepository<User> _userRepository,
                                   IRepository<UserVoucher> _userVoucherRepository) : ControllerBase
    {
        [HttpPost("create-voucher")]
        public async Task<IActionResult> CreateVoucher([FromBody] Voucher voucher)
        {
            Random random = new Random();
            string generatedCode;
            bool isDuplicate;

            do
            {
                generatedCode = Util.GenerateRandomString();
                isDuplicate = _voucherRepository.Find(v => v.Code == generatedCode).Any();
            } while (isDuplicate);

            Guid voucherID = Guid.NewGuid();
            Voucher createVoucher = new Voucher
            {
                Id = voucherID,
                Code = Util.GenerateRandomString(),
                Description = voucher.Description,
                Discount = voucher.Discount,
                StartDate = voucher.StartDate,
                EndDate = voucher.EndDate,
                Image = voucher.Image,
                isDeleted = false,
                QuantityUsed = voucher.QuantityUsed
            };
            await _voucherRepository.AddAsync(createVoucher);
            await _voucherRepository.SaveAsync();
            return Ok(new { Message = "Create Voucher Success" });
        }

        [HttpPut("edit-voucher/{id}")]
        public async Task<IActionResult> EditVoucher(Guid id, [FromBody] EditVoucherDTO voucherDto)
        {
            if (voucherDto == null)
            {
                return BadRequest(new { Message = "Invalid voucher data" });
            }

            var existingVoucher = await _voucherRepository.Find(v => v.Id == id).FirstOrDefaultAsync();

            if (existingVoucher == null)
            {
                return NotFound(new { Message = "Voucher not found" });
            }

            existingVoucher.Image = voucherDto.Image ?? existingVoucher.Image;
            existingVoucher.Discount = voucherDto.Discount;
            existingVoucher.Description = voucherDto.Description ?? existingVoucher.Description;
            existingVoucher.QuantityUsed = voucherDto.QuantityUsed;
            existingVoucher.StartDate = voucherDto.StartDate;
            existingVoucher.EndDate = voucherDto.EndDate;
            existingVoucher.isDeleted = voucherDto.isDeleted;

            await _voucherRepository.UpdateAsync(existingVoucher);
            await _voucherRepository.SaveAsync();

            return Ok(new { Message = "Voucher updated successfully" });
        }



        [HttpPost("receive")]
        public async Task<IActionResult> ReceiveVoucher([FromBody] ReceiveVoucherRequest request)
        {

            try
            {
                var user = await _userRepository.Find(u => u.Id == request.UserID).FirstOrDefaultAsync();
                var voucher = await _voucherRepository.Find(v => v.Id == request.VoucherID).FirstOrDefaultAsync();
                var checkAlready = await _userVoucherRepository.Find(x => x.UserID == request.UserID && request.VoucherID == request.VoucherID)
                                                                    .FirstOrDefaultAsync();
                if (checkAlready != null)
                {
                    return Conflict(new {Message = "You have already received this voucher" });
                }
                UserVoucher reciveVoucher = new UserVoucher
                {
                    isUsed = false,
                    UserID = user.Id,
                    VoucherID = voucher.Id,
                    voucher = voucher,
                    user = user,
                };
                await _userVoucherRepository.AddAsync(reciveVoucher);
                await _userVoucherRepository.SaveAsync();
                return Ok(new {Message = "Recive Voucher Success" });
            }
            catch (Exception ex) { 
            
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("list")]
        public async Task<IActionResult> GetVouchers([FromQuery] bool onlyValid = false)
        {
            var vouchersQuery = _voucherRepository.Find(v => !v.isDeleted);

            if (onlyValid)
            {
                vouchersQuery = vouchersQuery.Where(v => v.EndDate >= DateTime.UtcNow);
            }

            var vouchers = await vouchersQuery
                .Select(v => new
                {
                    VoucherID = v.Id,
                    Code = v.Code,
                    Description = v.Description,
                    Discount = v.Discount,
                    StartDate = v.StartDate,
                    EndDate = v.EndDate,
                    Image = v.Image
                })
                .ToListAsync();

            if (!vouchers.Any())
            {
                return NotFound(new { Message = "No vouchers found" });
            }

            return Ok(vouchers);
        }


        [HttpGet("user-vouchers")]
        public async Task<IActionResult> GetUserVouchers([FromHeader(Name = "X-User-Id")] Guid userId)
        {
            var user = await _userRepository.Find(u => u.Id == userId).FirstOrDefaultAsync();
            if (user == null)
            {
                return NotFound(new { Message = "User not found" });
            }

            var userVouchers = await _userVoucherRepository
                .Find(uv => uv.UserID == userId && !uv.isUsed && uv.voucher.EndDate >= DateTime.UtcNow && !uv.voucher.isDeleted)
                .Include(uv => uv.voucher)
                .Select(uv => new
                {
                    VoucherID = uv.VoucherID,
                    Code = uv.voucher.Code,
                    Description = uv.voucher.Description,
                    Discount = uv.voucher.Discount,
                    StartDate = uv.voucher.StartDate,
                    EndDate = uv.voucher.EndDate,
                    Image = uv.voucher.Image
                })
                .ToListAsync();

            if (!userVouchers.Any())
            {
                return NotFound(new { Message = "No valid vouchers found for this user" });
            }

            return Ok(userVouchers);
        }

    }
}
