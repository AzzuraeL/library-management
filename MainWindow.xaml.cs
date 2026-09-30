using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace LibraryManagementApp
{
    public partial class MainWindow : Window
    {
        private readonly ObservableCollection<Book> _books =
            new ObservableCollection<Book>();

        private readonly ICollectionView _view;

        private Book _editing = null;

        public MainWindow()
        {
            InitializeComponent();

            _view = CollectionViewSource.GetDefaultView(_books);
            _view.Filter = FilterBuku;
            lstBuku.ItemsSource = _view;

            _books.CollectionChanged += (s, e) => UpdateCounter();
            UpdateCounter();
        }

        // ================= EVENT 1: SIMPAN (CREATE / UPDATE) =================
        private void BtnSimpan_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtIsbn.Text))
            {
                MessageBox.Show("ISBN harus diisi!", "Peringatan",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtIsbn.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtJudul.Text))
            {
                MessageBox.Show("Judul Buku harus diisi!", "Peringatan",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtJudul.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPengarang.Text))
            {
                MessageBox.Show("Pengarang harus diisi!", "Peringatan",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtPengarang.Focus();
                return;
            }

            if (cmbKategori.SelectedItem == null)
            {
                MessageBox.Show("Pilih kategori buku!", "Peringatan",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (rbTersedia.IsChecked != true && rbDipinjam.IsChecked != true)
            {
                MessageBox.Show("Pilih status buku!", "Peringatan",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string isbn = txtIsbn.Text.Trim();

            if (_books.Any(b => b.Isbn == isbn && b != _editing))
            {
                MessageBox.Show("ISBN sudah terdaftar!", "Peringatan",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtIsbn.Focus();
                return;
            }

            if (dpTanggalTerbit.SelectedDate.HasValue &&
                dpTanggalTerbit.SelectedDate.Value.Date > DateTime.Today)
            {
                MessageBox.Show("Tanggal terbit tidak boleh di masa depan!", "Peringatan",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string kategori = "";
            if (cmbKategori.SelectedItem is ComboBoxItem item)
            {
                kategori = item.Content.ToString();
            }

            string status = rbTersedia.IsChecked == true ? "Tersedia" : "Dipinjam";

            var book = new Book
            {
                Isbn          = isbn,
                JudulBuku     = txtJudul.Text.Trim(),
                Pengarang     = txtPengarang.Text.Trim(),
                Kategori      = kategori,
                Status        = status,
                TanggalTerbit = dpTanggalTerbit.SelectedDate,
                Penerbit      = txtPenerbit.Text.Trim(),
                Deskripsi     = txtDeskripsi.Text.Trim()
            };

            if (_editing == null)
            {
                _books.Add(book);
                MessageBox.Show("Data buku berhasil disimpan!", "Informasi",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                int index = _books.IndexOf(_editing);
                _books[index] = book;
                MessageBox.Show("Data buku berhasil diperbarui!", "Informasi",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }

            ResetForm();
        }

        // ================= EVENT 2: RESET =================
        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            ResetForm();
        }

        private void ResetForm()
        {
            txtIsbn.Clear();
            txtJudul.Clear();
            txtPengarang.Clear();
            cmbKategori.SelectedIndex = -1;
            rbTersedia.IsChecked = false;
            rbDipinjam.IsChecked = false;
            dpTanggalTerbit.SelectedDate = null;
            txtPenerbit.Clear();
            txtDeskripsi.Clear();

            _editing = null;
            btnSimpan.Content = "Simpan";
            lstBuku.SelectedItem = null;

            txtIsbn.Focus();
        }

        // ================= EVENT 3: EDIT =================
        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (!(lstBuku.SelectedItem is Book b))
            {
                MessageBox.Show("Pilih data buku yang ingin diedit!", "Peringatan",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _editing = b;

            txtIsbn.Text      = b.Isbn;
            txtJudul.Text     = b.JudulBuku;
            txtPengarang.Text = b.Pengarang;

            cmbKategori.SelectedIndex = -1;
            foreach (ComboBoxItem cbi in cmbKategori.Items)
            {
                if (cbi.Content.ToString() == b.Kategori)
                {
                    cmbKategori.SelectedItem = cbi;
                    break;
                }
            }

            rbTersedia.IsChecked = b.Status == "Tersedia";
            rbDipinjam.IsChecked = b.Status == "Dipinjam";
            dpTanggalTerbit.SelectedDate = b.TanggalTerbit;
            txtPenerbit.Text  = b.Penerbit;
            txtDeskripsi.Text = b.Deskripsi;

            btnSimpan.Content = "Update";
            txtIsbn.Focus();
        }

        // ================= EVENT 4: HAPUS (DELETE) =================
        private void BtnHapus_Click(object sender, RoutedEventArgs e)
        {
            if (lstBuku.SelectedItem is Book b)
            {
                var result = MessageBox.Show(
                    $"Hapus data buku \"{b.JudulBuku}\" (ISBN: {b.Isbn})?",
                    "Konfirmasi",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    if (_editing == b)
                    {
                        ResetForm();
                    }
                    _books.Remove(b);
                }
            }
            else
            {
                MessageBox.Show("Pilih data buku yang ingin dihapus!", "Peringatan",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // ================= EVENT 5: SEARCH (TextChanged) =================
        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_view == null) return;
            _view.Refresh();
            UpdateCounter();
        }

        private bool FilterBuku(object obj)
        {
            var b = (Book)obj;
            string key = txtSearch.Text.Trim();

            if (key.Length == 0) return true;

            return b.Isbn.Contains(key, StringComparison.OrdinalIgnoreCase)
                || b.JudulBuku.Contains(key, StringComparison.OrdinalIgnoreCase)
                || b.Pengarang.Contains(key, StringComparison.OrdinalIgnoreCase)
                || b.Kategori.Contains(key, StringComparison.OrdinalIgnoreCase);
        }

        // ================= COUNTER =================
        private void UpdateCounter()
        {
            int total = _books.Count;
            int shown = _view.Cast<object>().Count();

            lblJumlah.Text = shown == total
                ? $"Jumlah Buku: {total}"
                : $"Jumlah Buku: {total} (ditampilkan {shown})";
        }
    }
}
