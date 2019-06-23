namespace DocumentStorage
{
    public class DocumentStorageMetadata
    {
        private readonly DocumentCollectionMetadata[] collectionMetadataList;

        public DocumentStorageMetadata(DocumentCollectionMetadata[] collectionMetadataList)
        {
            this.collectionMetadataList = collectionMetadataList;
        }

        public DocumentMetadata this[DocumentId docId] 
            => collectionMetadataList[docId.CollectionId][docId];
    }
}
