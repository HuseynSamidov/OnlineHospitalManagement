namespace Application.Shared;

public static class Permissions
{
    public static class QueueTicket
    {
        public const string Create = "QueueTicket.Create";
        public const string GetAll = "QueueTicket.GetAll";
        public const string GetDetail = "QueueTicket.GetDetail";
        public const string Delete = "QueueTicket.Delete";
        public const string Update = "QueueTicket.Update";
        public const string GetMy = "QueueTicket.GetMy";

        public static List<string> All = new()
        {
            Create,
            GetAll,
            GetDetail,
            Delete,
            Update,
            GetMy
        };
    }
    public static class User
    {
        public const string Register = "User.Register";
        public const string Login = "User.Login";
        public const string Update = "User.Update";
        public const string ResetPassword = "User.ResetPassword";
        public const string ConfirmEmail = "User.ConfirmEmail";
        public const string GetById = "User.GetById";
        public const string GetAll = "User.GetAll";

        public static List<string> All = new()
        {
            Register,
            Login,
            Update,
            ResetPassword,
            ConfirmEmail,
            GetById,
            GetAll
        };
    }
    public static class Category
    {
        public const string MainCreate = "Category.MainCreate";
        public const string SubCreate = "Category.SubCreate";
        public const string MainUpdate = "Category.MainUpdate";
        public const string Delete = "Category.Delete";
        public const string GetAll = "Category.GetAll";
        public const string GetDetail = "Category.GetDetail";

        public static List<string> All = new()
        {
            MainCreate,
            SubCreate,
            MainUpdate,
            Delete,
            GetAll,
            GetDetail
        };

    }

}
