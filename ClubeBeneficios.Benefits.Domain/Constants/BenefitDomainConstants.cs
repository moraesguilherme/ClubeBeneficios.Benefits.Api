namespace ClubeBeneficios.Benefits.Domain.Constants;

public static class BenefitDomainConstants
{
    public static class Direction
    {
        public const string PartnerToMatilha = "partner_to_matilha";
        public const string MatilhaToPartner = "matilha_to_partner";

        public static readonly string[] All =
        [
            PartnerToMatilha,
            MatilhaToPartner
        ];
    }

    public static class TargetActorType
    {
        public const string Client = "client";
        public const string PartnerCustomer = "partner_customer";

        public static readonly string[] All =
        [
            Client,
            PartnerCustomer
        ];
    }

    public static class Status
    {
        public const string Draft = "draft";
        public const string PendingReview = "pending_review";
        public const string UnderReview = "under_review";
        public const string Approved = "approved";
        public const string Active = "active";
        public const string Inactive = "inactive";
        public const string Rejected = "rejected";
        public const string Archived = "archived";

        public static readonly string[] All =
        [
            Draft,
            PendingReview,
            UnderReview,
            Approved,
            Active,
            Inactive,
            Rejected,
            Archived
        ];
    }

    public static class EligibilityType
    {
        public const string Open = "open";
        public const string Level = "level";
        public const string Behavior = "behavior";
        public const string Code = "code";
        public const string Hybrid = "hybrid";
        public const string Custom = "custom";

        public static readonly string[] All =
        [
            Open,
            Level,
            Behavior,
            Code,
            Hybrid,
            Custom
        ];
    }

    public static class LevelType
    {
        public const string PartnerLevel = "partner_level";
        public const string ClientLevel = "client_level";

        public static readonly string[] All =
        [
            PartnerLevel,
            ClientLevel
        ];
    }

    public static class LevelCode
    {
        public const string Bronze = "bronze";
        public const string Silver = "silver";
        public const string Gold = "gold";
        public const string Diamond = "diamond";
        public const string Platinum = "platinum";

        public static readonly string[] All =
        [
            Bronze,
            Silver,
            Gold,
            Diamond,
            Platinum
        ];
    }

    public static class RecurrenceType
    {
        public const string OncePerCustomer = "once_per_customer";
        public const string Unlimited = "unlimited";
        public const string Periodic = "periodic";

        public static readonly string[] All =
        [
            OncePerCustomer,
            Unlimited,
            Periodic
        ];
    }

    public static class RecurrencePeriod
    {
        public const string Week = "week";
        public const string Month = "month";
        public const string Quarter = "quarter";
        public const string Semester = "semester";
        public const string Year = "year";

        public static readonly string[] All =
        [
            Week,
            Month,
            Quarter,
            Semester,
            Year
        ];
    }

    public static class ValidityType
    {
        public const string Continuous = "continuous";
        public const string FixedPeriod = "fixed_period";

        public static readonly string[] All =
        [
            Continuous,
            FixedPeriod
        ];
    }

    public static class CodeValidationMode
    {
        public const string PartnerCode = "partner_code";
        public const string MatilhaCoupon = "matilha_coupon";
        public const string InviteCode = "invite_code";

        public static readonly string[] All =
        [
            PartnerCode,
            MatilhaCoupon,
            InviteCode
        ];
    }
}