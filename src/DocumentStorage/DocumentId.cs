using System;

namespace DocumentStorage
{
    /// <summary>
    /// DocumentId is an unique identifier of documents in a storage,
    /// consists of 2 parts: if of the collections 
    /// and local id which is the document's id in the collection
    /// </summary>
    public readonly struct DocumentId : IComparable<DocumentId>
    {
        private const int offset = 16;

        public static readonly DocumentId Zero = new DocumentId(0, 0);

        public readonly uint Id;

        public ushort CollectionId => (ushort)(Id >> offset);

        public ushort LocalId => (ushort)Id;

        public DocumentId(uint id) => Id = id;

        public DocumentId(ushort collectionId, ushort localId)
        {
            Id = ((uint)collectionId << offset) + localId;
        }

        public int CompareTo(DocumentId other) => Id.CompareTo(other.Id);

        public override string ToString() => string.Format($"{Id:X8}");

        public string CollectionIdString() => string.Format($"{CollectionId:X4}");

        public string LocalIdString() => string.Format($"{LocalId:X4}");

        public static DocumentId Parse(string hex)
        {
            return new DocumentId(Convert.ToUInt32(hex, 16));
        }

        public static ushort ParseCollectionId(string hex) => Convert.ToUInt16(hex, 16);

        public static ushort ParseLocalId(string hex) => Convert.ToUInt16(hex, 16);
    }
}
