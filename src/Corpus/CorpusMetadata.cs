namespace Corpus
{
    public class CorpusMetadata
    {
        private readonly BlockMetadata[] blockMetadataList;

        public CorpusMetadata(BlockMetadata[] blockMetadataList)
        {
            this.blockMetadataList = blockMetadataList;
        }

        public DocumentMetadata this[DocumentId docId] 
            => blockMetadataList[docId.BlockId][docId];
    }
}
