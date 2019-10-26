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
using System.Diagnostics;

using Corpus;
using InformationRetrieval.BooleanSearch;
using InformationRetrieval.Indexing;
using InformationRetrieval.Indexing.External;
using InformationRetrieval.Indexing.InMemory;
using InformationRetrieval.Utility;

namespace InformationRetrieval.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static readonly string basePath = @"F:\wikipedia";
        static readonly string wikiPath = IO::Path.Combine(basePath, "enwiki");
        static readonly string indexPath = IO::Path.Combine(basePath, "index.bin");
        static readonly string externalIndexPath = IO::Path.Combine(basePath, "external_index");

        private readonly Lazy<BooleanSearchEngine<int>> engine;
        private readonly Lazy<CorpusMetadata> metadata;
        private readonly CorpusZipReader<string> reader;

        private bool useExternalIndex = true;
        
        Stopwatch timer = new Stopwatch();

        public MainWindow()
        {
            InitializeComponent();

            engine = new Lazy<BooleanSearchEngine<int>>(LoadSearchEngine);
            reader = new CorpusZipReader<string>(wikiPath, new StringDocumentDataSerializer());
            metadata = new Lazy<CorpusMetadata>(LoadMetadata);
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                timer.Restart();
                var results = await ExecuteQuery(QueryTextBox.Text);
                timer.Stop();
                Log($"Search of '{QueryTextBox.Text}' took {timer.Elapsed:g}, found {results.Length} documents");
                DocumentIDsListBox.ItemsSource = results.Select(id => new DocumentIdTemplate(id, metadata.Value));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void DocumentIDsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                timer.Restart();
                var docId = ((DocumentIdTemplate)e.AddedItems[0]).Id;
                var doc = reader.ReadDocument(docId, skipMetadata: true);
                timer.Stop();
                Log($"Document '{docId} - {metadata.Value[docId]?.Title}' loaded in {timer.Elapsed:g}");
                DocumentTextBox.Text = doc.Data;
            }
            else
            {
                DocumentTextBox.Text = "";
            }
        }

        private async Task<DocumentId[]> ExecuteQuery(string query)
        {
            var booleanQuery = BooleanQueryLanguage.ParseQuery(query);
            DocumentId[] documentId = await Task.Run(() =>
               {
                   lock (engine)
                   {
                       return engine.Value.ExecuteQuery(booleanQuery).ToArray();
                   }
               });
            return documentId;
        }

        private void Log(string message)
        {
            var record = new { Time = DateTime.Now, Message = message };
            Dispatcher.Invoke(() => 
            {
                LogListView.Items.Add(record);
                LogListView.ScrollIntoView(record);
            });
        }

        private class DocumentIdTemplate
        {
            private readonly string title;

            public DocumentIdTemplate(DocumentId id, CorpusMetadata metadata)
            {
                Id = id;
                title = metadata[id].Title;
            }

            public DocumentId Id { get; }

            public override string ToString() => title;
        }

        private ISearchableIndex<int> LoadInMemoryIndex()
        {
            using var file = IO::File.OpenRead(indexPath);
            return DictionaryIndex<int>.Deserialize(file);
        }

        private ISearchableIndex<int> LoadExternalIndex()
        {
            var serializer = new ExternalIndexSerializer<int>();
            return serializer.Deserialize(externalIndexPath);
        }

        private ISearchableIndex<int> LoadIndex()
        {
            if (useExternalIndex)
            {
                return LoadExternalIndex();
            }
            else
            {
                return LoadInMemoryIndex();
            }
        }

        private BooleanSearchEngine<int> LoadSearchEngine()
        {
            var timer = new Stopwatch();
            timer.Start();
            var index = LoadIndex();
            timer.Stop();
            Log($"Index loaded in {timer.Elapsed:g}");

            return new BooleanSearchEngine<int>(index, s => TextHasher.CalculateHashCode(s.ToLower().AsSpan()));
        }

        private CorpusMetadata LoadMetadata()
        {
            var timer = new Stopwatch();
            timer.Start();
            var metadata = reader.ReadMetadata();
            timer.Stop();
            Log($"Metadata loaded in {timer.Elapsed:g}");
            return metadata;
        }
    }
}
