using EazyMenu.Application.Common.Interfaces;
using EazyMenu.Application.Common.Models.Auth;
using EazyMenu.Domain.Entities;
using MediatR;

namespace EazyMenu.Application.Features.Auth.Commands.VerifyOtp;

/// <summary>
/// Handler برای تایید کد OTP و ورود کاربر
/// </summary>
public class VerifyOtpCommandHandler : IRequestHandler<VerifyOtpCommand, AuthResult>
{
    private readonly IRepository<ApplicationUser> _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOtpService _otpService;

    public VerifyOtpCommandHandler(
        IRepository<ApplicationUser> userRepository,
        IUnitOfWork unitOfWork,
        IOtpService otpService)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _otpService = otpService;
    }

    public async Task<AuthResult> Handle(VerifyOtpCommand request, CancellationToken cancellationToken)
    {
        // بررسی صحت کد OTP
        var isValid = await _otpService.VerifyOtpAsync(request.PhoneNumber, request.Code, cancellationToken);
        
        if (!isValid)
        {
            return new AuthResult
            {
                Success = false,
                Message = "کد تایید اشتباه یا منقضی شده است"
            };
        }

        // حذف کد از Cache (استفاده یکباره)
        await _otpService.RemoveOtpAsync(request.PhoneNumber, cancellationToken);

        // جستجوی کاربر
        var users = await _userRepository.FindAsync(
            u => u.PhoneNumber == request.PhoneNumber && !u.IsDeleted,
            cancellationToken);

        var user = users.FirstOrDefault();

        if (user == null)
        {
            return new AuthResult
            {
                Success = false,
                Message = "کاربری با این شماره موبایل یافت نشد. لطفاً ابتدا ثبت‌نام کنید"
            };
        }

        // بررسی فعال بودن کاربر
        if (!user.IsActive)
        {
            return new AuthResult
            {
                Success = false,
                Message = "حساب کاربری شما غیرفعال شده است"
            };
        }

        // تایید شماره موبایل
        if (!user.PhoneNumberConfirmed)
        {
            user.PhoneNumberConfirmed = true;
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        // به‌روزرسانی زمان آخرین ورود
        user.LastLoginAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // ورود موفق
        return new AuthResult
        {
            Success = true,
            Message = "ورود موفقیت‌آمیز با کد تایید",
            User = new UserInfoDto
            {
                Id = user.Id,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                ProfileImageUrl = user.ProfileImageUrl,
                PreferredLanguage = user.PreferredLanguage,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            }
        };
    }
}
