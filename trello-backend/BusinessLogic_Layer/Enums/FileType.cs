
namespace BusinessLogic_Layer.Enums
{
    public enum FileType
    {
        // Các loại file có thể upload
        Txt = 1,
        Doc,
        Docx,
        Xls,
        Xlsx,
        Pdf,
        Png,
        Jpg,
        Jpeg,
        Gif,
        Zip,
        Rar,

        // Các loại file hệ thống không cho phép upload
        Sys = 100,
        Dll,
        Exe,
        Bat,
        Cmd,
        Com,
        Bin,
        Msi
    }
}
