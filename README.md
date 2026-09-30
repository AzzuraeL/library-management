Step 1 : Membuat Project

Buka terminal di VS Code, buat project WPF baru menggunakan perintah:

dotnet new wpf -n LibraryManagementApp

cd LibraryManagementApp

code .



Step 2 : Struktur Project

Setelah project dibuat, perhatikan panel Explorer di VS Code. Struktur file project:

LibraryManagementApp

├──App.xaml

├── App.xaml.cs

├── MainWindow.xaml

├── MainWindow.xaml.cs

├── Book.cs

└── LibraryManagementApp.csproj

Source Code dapat ditemukan di : LIBRARY MANAGEMENT



Step 3 : Build Project

Setelah semua file selesai, build project menggunakan perintah:

dotnet build

Jika berhasil, output terminal menampilkan:

LibraryManagementApp succeeded (0,3s) → bin\Debug\net8.0-windows\LibraryManagementApp.dll

Build succeeded in 1,0s



Step 4 : Jalankan Aplikasi

Jalankan aplikasi menggunakan perintah:

dotnet run
