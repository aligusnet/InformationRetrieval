using System;
using System.Collections.Generic;
using IO = System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DocumentStorage;
using NaturalLanguageTools.BooleanSearch;
using NaturalLanguageTools.Indexing;
using System.Diagnostics;

namespace NaturalLanguageTools.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static readonly string basePath = @"F:\wikipedia";
        static readonly string wikiPath = IO::Path.Combine(basePath, "enwiki");
        static readonly string indexPath = IO::Path.Combine(basePath, "index.bin");

        private readonly BooleanSearchEngine<int> engine;
        private readonly StorageZipReader<string> reader;
        private readonly DocumentStorageMetadata metadata;
        Stopwatch timer = new Stopwatch();

        public MainWindow()
        {
            InitializeComponent();

            using var file = IO::File.OpenRead(indexPath);
            timer.Restart();
            var index = DictionaryIndex<int>.Deserialize(file);
            timer.Stop();
            Log($"Index loaded in {timer.Elapsed:g}");

            engine = new BooleanSearchEngine<int>(index, s => DocumentHasher.CalculateHashCode(s.ToLower().AsSpan()));

            reader = new StorageZipReader<string>(wikiPath, new StringDocumentDataSerializer());

            timer.Restart();
            metadata = reader.ReadMetadata();
            timer.Stop();
            Log($"Metadata loaded in {timer.Elapsed:g}");
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                timer.Restart();
                var query = BooleanQueryLanguage.ParseQuery(QueryTextBox.Text);
                var results = engine.ExecuteQuery(query).ToList();
                timer.Stop();
                Log($"Search of '{QueryTextBox.Text}' took {timer.Elapsed:g}, found {results.Count} documents");
                DocumentIDsListBox.ItemsSource = results.Select(id => new DocumentIdTemplate(id, metadata));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DocumentIDsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                timer.Restart();
                var docId = ((DocumentIdTemplate)e.AddedItems[0]).Id;
                var doc = reader.ReadDocument(docId);
                timer.Stop();
                Log($"Document '{docId} - {doc.Metadata.Title}' loaded in {timer.Elapsed:g}");
                DocumentTextBox.Text = doc.Data;
            }
            else
            {
                DocumentTextBox.Text = "";
            }
        }

        private void Log(string message)
        {
            var record = new { Time = DateTime.Now, Message = message };
            LogListView.Items.Add(record);
            LogListView.ScrollIntoView(record);
        }

        private class DocumentIdTemplate
        {
            private readonly string title;

            public DocumentIdTemplate(DocumentId id, DocumentStorageMetadata metadata)
            {
                Id = id;
                title = metadata[id].Title;
            }

            public DocumentId Id { get; }

            public override string ToString() => title;
        }
    }
}
