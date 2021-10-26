using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CommonLayer.Custom
{
    public class Result
    {
        [NotMapped]
        public ResultCrud result { get; set; } = ResourceFa.Success;
    }
    public class ResultCrud
    {
        [NotMapped]
        public string message { get; set; }

        //[NotMapped]
        //public bool error { get { return code >= 300 && code <= 399; } set { error = value; } }

        [NotMapped]
        public int code { get; set; } = 200;
        [NotMapped]
        public dynamic data { get; set; }

    }

    public sealed class ResourceFa
    {

        //public static string Success = $"عملیات با موفقیت انجام شد";
        //public static string SuccessChangeUserName = $"تغییر نام کاربری با موفقیت انجام شد";
        //public static string SuccessChangePassword = $"ایجاد کلمه عبور جدید و ارسال آن با موفقیت انجام شد";
        //public static string SuccessSendAcceptCode = $"کد پیگیری عملیات با موفقیت پیامک شد";
        //public static string SaveDataSuccess = $"ثبت موفق داده";
        //public static string SaveDataError = $"بروز خطا در ثبت داده";
        //public static string Error = $"بروز خطا در انجام عملیات";
        //public static string DeleteSuccess = $"حذف با موفقیت انجام شد";
        //public static string DeleteError = $"بروز خطا در حذف داده";
        //public static string DeleteForChildError = $"این رکورد به دلیل داشتن رکوردهای مرتبط قابل حذف  نمی باشد. ابتدا رکوردهای مرتبط را حذف نمایید.";
        //public static string DeleteDataExistScorePropertyError = $"ثبت داده با موفقیت انجام شد , چنانچه مشخصه ای را حذف کرده اید و همچنان وجود دارد , دلیل آن وجود امتیاز برای آن است";
        //public static string SaveScoreSuccess = $"ثبت موفق امتیاز";
        //public static string ErrorUserNotExist = $"کاربری با این مشخصات وجود ندارد !";
        //public static string ErrorTokenNotExist = $"مجوز ارسالی نامعتبر است !";
        //public static string ErrorUserDuplicated = $"نام کاربری وارد شده تکراری است !";
        //public static string ErrorRenovationCodeDuplicated = $"/*کد نوسازی وارد شده قبلا ثبت شده است*/ !";
        //public static string ErrorSellerPlaceDuplicated = $"این کاربر قبلا برای این مکان بعنوان فروشنده معرفی شده !";
        //public static string SuccessUserInitData = $"مقداردهی اولیه اطلاعات کاربر با موفقیت انجام شد";
        //public static string SuccessUserAuthorization = $"ثبت نام یا تعیین هویت کاربر با موفقیت انجام شد";
        //public static string ErrorUserAuthorization = $"خطا در ثبت نام یا نعیین هویت کاربر !";
        //public static string SuccessUserPlaceFollow = $"فعال/غیرفعال سازی دنبال کننده این مکان با موفقیت انجام شد";
        //public static string ErrorUserPlaceFollow = $"خطا در فعال/غیرفعال سازی دنبال کننده !";
        //public static string SuccessUserProductFolder = $"ثبت کالا در پوشه مربوطه با موفقیت انجام شد";
        //public static string ErrorUserProductFolder = $"خطا در ثبت کالا در پوشه مربوطه !";
        //public static string SuccessUserProductLike = $"فعال/غیرفعال سازی علاقه مندی این کالا با موفقیت انجام شد";
        //public static string ErrorUserProductLike = $"خطا در فعال/غیرفعال سازی علاقه مندی کالا !";
        //public static string SuccessBatchMoveFolder = $"جابجایی آیتمهای انتخابی در پوشه با موفقیت انجام شد";

        public static ResultCrud DeleteCode => new ResultCrud { code = 600 };
        public static ResultCrud AgoExist => new ResultCrud { code = 700 };
        public static ResultCrud Success => new ResultCrud { message = $"عملیات با موفقیت انجام شد", code = 200 };
        public static ResultCrud SuccessChangeUserName => new ResultCrud { message = $"تغییر نام کاربری با موفقیت انجام شد", code = 201 };
        public static ResultCrud SuccessChangePassword => new ResultCrud { message = $"ایجاد کلمه عبور جدید و ارسال آن با موفقیت انجام شد", code = 202 };
        public static ResultCrud SuccessSendAcceptCode => new ResultCrud { message = $"کد پیگیری عملیات با موفقیت پیامک شد", code = 203 };
        public static ResultCrud SuccessSaveData => new ResultCrud { message = $"ثبت موفق داده", code = 204 };
        public static ResultCrud ErrorSaveData => new ResultCrud { message = $"بروز خطا در ثبت داده", code = 301 };
        public static ResultCrud Error => new ResultCrud { message = $"بروز خطا در انجام عملیات", code = 300 };
        public static ResultCrud DeleteSuccess => new ResultCrud { message = $"حذف با موفقیت انجام شد", code = 202 };
        public static ResultCrud ActiveDeactiveSuccess => new ResultCrud { message = $"فعال/غیرفعال با موفقیت انجام شد", code = 202 };
        public static ResultCrud AcceptedSuccess => new ResultCrud { message = $"تایید محصول با موفقیت انجام شد", code = 202 };
        public static ResultCrud FixedNotFixedSuccess => new ResultCrud { message = $"رفع اشکال/عدم رفع اشکال با موفقیت انجام شد", code = 202 };
        public static ResultCrud DeleteError => new ResultCrud { message = $"بروز خطا در حذف داده", code = 303 };
        public static ResultCrud ActiveDeactiveError => new ResultCrud { message = $"بروز خطا در فعال/غیرفعال داده", code = 303 };
        public static ResultCrud AcceptedError => new ResultCrud { message = $"این محصول قبلا تایید شده است", code = 303 };
        public static ResultCrud AcceptedRejectListError => new ResultCrud { message = $"این محصول دارای اشکالات رفع نشده است و برای تایید باید ابتدا اشکالات بصورت کامل رفع شوند", code = 304 };
        public static ResultCrud SuccessSaveScore => new ResultCrud { message = $"ثبت موفق امتیاز", code = 205 };
        public static ResultCrud ErrorUserNotExist => new ResultCrud { message = $"کاربری با این مشخصات وجود ندارد !", code = 305 };
        public static ResultCrud ErrorUserDuplicated => new ResultCrud { message = $"نام کاربری وارد شده تکراری است !", code = 307 };
        public static ResultCrud SuccessUserInitData => new ResultCrud { message = $"مقداردهی اولیه اطلاعات کاربر با موفقیت انجام شد", code = 206 };
        public static ResultCrud SuccessUserAuthorization => new ResultCrud { message = $"ثبت نام یا تعیین هویت کاربر با موفقیت انجام شد", code = 207 };
        public static ResultCrud ErrorUserAuthorization => new ResultCrud { message = $"خطا در ثبت نام یا نعیین هویت کاربر !", code = 208 };
        public static ResultCrud SuccessUserPlaceFollow => new ResultCrud { message = $"فعال/غیرفعال سازی دنبال کننده این مکان با موفقیت انجام شد", code = 209 };
        public static ResultCrud ErrorTokenNotExist => new ResultCrud { message = $"مجوز ارسالی نامعتبر است !", code = 306 };

        public static ResultCrud MaxLimitFileUpload => new ResultCrud { message = "حداکثر حجم فایل 200 کیلوبایت می‌باشد.", code = 400 };

        public static ResultCrud ErrorSameTransactionAgoRuning => new ResultCrud { message = $"این درخواست توسط اپراتور دیگری در حال اجرا می باشد!", code = 700 };

        public static ResultCrud ErrorInvalidFormat => new ResultCrud { message = $"فرمت ورودی در یکی از فیلدها نامعتبر است!", code = 710 };

        public static string DeleteForChildError = "این رکورد به دلیل داشتن رکوردهای مرتبط در بخش‌های <b>{0}</b> قابل حذف نمی‌باشد.";

        #region WarehoseErrors

        public static string ErrorCountInputWarehouse = $"تعداد ورودی نباید کمتر از تعداد خارج شده از انبار باشد";
        public static string ErrorCountOutputWarehouse = $"تعداد خارج شده از تعداد فروش شده کمتر می باشد";
        public static string ErrorCountReamindWarehouse = $"تعداد خروجی از موجودی انبار بیشتر می باشد";
        #endregion WareHoseErrors

        #region CategoryErrors

        public static string ErrorCategoryImageType = $"انتخاب عکس برای دسته بندی الزامی ست";
        public static string ErrorCategoryTitle = $"انتخاب نام برای دسته بندی الزامی ست";
        public static string ErrorCategoryImageActive = $"انتخاب یک عکس در دسته بندی به عنوان عکس فعال، الزامی ست";

        #endregion CategoryErrors

        #region ProductErrors

        public static string ErrorProductImage = $"انتخاب عکس برای محصول الزامی ست";
        public static string ErrorProductTitle = $"انتخاب نام برای محصول الزامی ست";
        public static string ErrorProductDescription = $"نوشتن توضیحات برای محصول الزامی ست";
        public static string ErrorProductExist = $"محصول از قبل تعریف شده است";
        public static string ErrorCategoryExist = $"دسته بندی از قبل تعریف شده است";

        public static string ProductError = "محصول تکراری می باشد";
        #endregion ProductErrors

        #region PageComponentErrors

        public static string ErrorComponentImage = $"انتخاب عکس برای  کامپوننت الزامی ست";
        public static string ErrorComponentTitle = $"انتخاب نام برای کامپوننت  الزامی ست";


        #endregion PageComponentErrors

        #region PageComponentGroupErrors

        public static string ErrorComponentGroupImage = $"انتخاب عکس برای  محصول الزامی ست";
        public static string ErrorComponentGroupTitle = $"انتخاب نام برای محصول  الزامی ست";


        #endregion PageComponentGroupErrors
    }
}
