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
            => blockMetadataList[GetBlockId(docId)][docId];

        private int GetBlockId(DocumentId docId)
        {
            int blockId;
            for (blockId = 1; blockId < blockMetadataList.Length; ++blockId)
            {
                if (blockMetadataList[blockId].First().Id.CompareTo(docId) > 0)
                {
                    break;
                }
            }
            return blockId - 1;
        }
    }
}
