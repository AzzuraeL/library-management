using System;

namespace LibraryManagementApp
{
    // Model data buku untuk Library Management App
    public class Book
    {
        public string Isbn { get; set; }
        public string JudulBuku { get; set; }
        public string Pengarang { get; set; }
        public string Kategori { get; set; }
        public string Status { get; set; }
        public DateTime? TanggalTerbit { get; set; }
        public string Penerbit { get; set; }
        public string Deskripsi { get; set; }

        // Dipakai oleh ItemTemplate di ListBox
        public string Judul => $"{Isbn} | {JudulBuku}";

        public string Detail =>
            $"{Pengarang} | {Kategori} | {Status} | " +
            $"{(TanggalTerbit.HasValue ? TanggalTerbit.Value.ToString("dd/MM/yyyy") : "-")} | " +
            $"{(string.IsNullOrWhiteSpace(Penerbit) ? "-" : Penerbit)}";
    }
}
