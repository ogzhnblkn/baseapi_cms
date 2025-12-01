namespace BaseApi.Application.Common
{
    public static class Messages
    {
        public static class Page
        {
            public const string Created = "Sayfa baþarýyla oluþturuldu";
            public const string Updated = "Sayfa baþarýyla güncellendi";
            public const string Deleted = "Sayfa baþarýyla silindi";
            public const string Retrieved = "Sayfa baþarýyla getirildi";
            public const string ListRetrieved = "Sayfalar baþarýyla getirildi";
            public const string NotFound = "Sayfa bulunamadý";
            public const string SlugExists = "Bu slug zaten kullanýmda";
            public const string ValidationFailed = "Sayfa doðrulama hatasý";
        }

        public static class Product
        {
            public const string Created = "Product created successfully";
            public const string Updated = "Product updated successfully";
            public const string Deleted = "Product deleted successfully";
            public const string Retrieved = "Product retrieved successfully";
            public const string ListRetrieved = "Products retrieved successfully";
            public const string NotFound = "Product not found";
            public const string SlugExists = "A product with this slug already exists";
            public const string ValidationFailed = "Product validation failed";
        }

        public static class Menu
        {
            public const string Created = "Menu created successfully";
            public const string Updated = "Menu updated successfully";
            public const string Deleted = "Menu deleted successfully";
            public const string Retrieved = "Menu retrieved successfully";
            public const string ListRetrieved = "Menus retrieved successfully";
            public const string NotFound = "Menu not found";
            public const string SlugExists = "A menu with this slug already exists";
            public const string HasChildren = "Cannot delete menu that has child menus";
        }

        public static class Slider
        {
            public const string Created = "Slider created successfully";
            public const string Updated = "Slider updated successfully";
            public const string Deleted = "Slider deleted successfully";
            public const string Retrieved = "Slider retrieved successfully";
            public const string ListRetrieved = "Sliders retrieved successfully";
            public const string NotFound = "Slider not found";
        }

        public static class User
        {
            public const string Created = "User created successfully";
            public const string Updated = "User updated successfully";
            public const string Deleted = "User deleted successfully";
            public const string Retrieved = "User retrieved successfully";
            public const string ListRetrieved = "Users retrieved successfully";
            public const string NotFound = "User not found";
            public const string UsernameExists = "Username already exists";
            public const string EmailExists = "Email already exists";
            public const string UserExists = "User already exists";
        }

        public static class Auth
        {
            public const string LoginSuccess = "Login successful";
            public const string RegisterSuccess = "Registration successful";
            public const string InvalidCredentials = "Invalid username or password";
            public const string UserExists = "User already exists";
            public const string Unauthorized = "User not authenticated";
        }

        public static class File
        {
            public const string Uploaded = "File uploaded successfully";
            public const string Deleted = "File deleted successfully";
            public const string NotFound = "File not found";
            public const string UploadFailed = "File upload failed";
            public const string InvalidFormat = "Invalid file format";
            public const string TooLarge = "File size too large";
        }

        public static class General
        {
            public const string Success = "Operation completed successfully";
            public const string Error = "An error occurred";
            public const string ValidationFailed = "Validation failed";
            public const string NotFound = "Resource not found";
            public const string Unauthorized = "Unauthorized access";
            public const string Forbidden = "Access forbidden";
            public const string Conflict = "Resource conflict";
        }

        public static class SocialMediaLink
        {
            public const string Created = "Sosyal medya linki baþarýyla oluþturuldu";
            public const string Updated = "Sosyal medya linki baþarýyla güncellendi";
            public const string Deleted = "Sosyal medya linki baþarýyla silindi";
            public const string Retrieved = "Sosyal medya linki baþarýyla getirildi";
            public const string ListRetrieved = "Sosyal medya linkleri baþarýyla getirildi";
            public const string NotFound = "Sosyal medya linki bulunamadý";
            public const string ValidationFailed = "Sosyal medya linki doðrulama hatasý";
        }
    }
}