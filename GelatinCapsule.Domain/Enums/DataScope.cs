namespace GelatinCapsule.Domain.Enums;

public enum DataScope : byte
{
    OnlyOwn = 0,                    // فقط رکوردهای خودش
    Team = 1,                       // رکوردهای تیمش
    DepartmentAndChildren = 2,      // واحد خودش + زیرمجموعه‌ها
    All = 3                         // همه چیز
}