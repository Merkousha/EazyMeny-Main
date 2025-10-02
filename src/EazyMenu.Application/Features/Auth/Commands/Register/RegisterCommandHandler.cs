using EazyMenu.Application.Common.Interfaces;
using EazyMenu.Application.Common.Models.Auth;
using EazyMenu.Domain.Entities;
using MediatR;

namespace EazyMenu.Application.Features.Auth.Commands.Register;

/// <summary>
/// Handler برای ثبت‌نام کاربر جدید
/// </summary>
public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResult>
{
    private readonly IRepository<ApplicationUser> _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasherService _passwordHasher;
    private readonly ISmsService _smsService;

    public RegisterCommandHandler(
        IRepository<ApplicationUser> userRepository,
        IUnitOfWork unitOfWork,
        IPasswordHasherService passwordHasher,
        ISmsService smsService)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _smsService = smsService;
    }

    public async Task<AuthResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        // بررسی تکراری بودن شماره موبایل
        var existingUserByPhone = await _userRepository.FindAsync(
            u => u.PhoneNumber == request.PhoneNumber && !u.IsDeleted,
            cancellationToken);
        
        if (existingUserByPhone.Any())
        {
            return new AuthResult
            {
                Success = false,
                Message = "این شماره موبایل قبلاً ثبت شده است"
            };
        }

        // بررسی تکراری بودن ایمیل (اگر وارد شده)
        if (!string.IsNullOrEmpty(request.Email))
        {
            var existingUserByEmail = await _userRepository.FindAsync(
                u => u.Email == request.Email && !u.IsDeleted,
                cancellationToken);
            
            if (existingUserByEmail.Any())
            {
                return new AuthResult
                {
                    Success = false,
                    Message = "این ایمیل قبلاً ثبت شده است"
                };
            }
        }

        // ایجاد کاربر جدید
        var user = new ApplicationUser
        {
            UserName = request.PhoneNumber, // Username = PhoneNumber
            FullName = request.FullName,
            PhoneNumber = request.PhoneNumber,
            Email = request.Email ?? string.Empty,
            IsActive = true,
            PhoneNumberConfirmed = false, // باید با OTP تایید شود
            EmailConfirmed = false,
            PreferredLanguage = "fa"
        };

        // Hash کردن رمز عبور
        user.PasswordHash = _passwordHasher.HashPassword(request.Password);

        // ذخیره کاربر
        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // ارسال پیامک خوش‌آمدگویی (اختیاری)
        try
        {
            await _smsService.SendAsync(
                user.PhoneNumber,
                $"سلام {user.FullName} عزیز، به ایزی‌منو خوش آمدید!",
                cancellationToken);
        }
        catch
        {
            // اگر ارسال SMS ناموفق بود، مشکلی نیست
        }

        return new AuthResult
        {
            Success = true,
            Message = "ثبت‌نام با موفقیت انجام شد. لطفاً وارد شوید",
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
