using System;

namespace Corpus
{
    /// <summary>
    /// DocumentId is an unique identifier of documents in corpus,
    /// consists of 2 parts: id of the block
    /// and local id which is the document's id in the block
    /// </summary>
    public readonly struct DocumentId : IComparable<DocumentId>
    {
        private const int offset = 16;

        public static readonly DocumentId Zero = new DocumentId(0, 0);

        public readonly uint Id;

        public ushort BlockId => (ushort)(Id >> offset);

        public ushort LocalId => (ushort)Id;

        public DocumentId(uint id) => Id = id;

        public DocumentId(ushort blockId, ushort localId)
        {
            Id = ((uint)blockId << offset) + localId;
        }

        public int CompareTo(DocumentId other) => Id.CompareTo(other.Id);

        public override string ToString() => string.Format($"{Id:X8}");

        public string BlockIdString() => string.Format($"{BlockId:X4}");

        public string LocalIdString() => string.Format($"{LocalId:X4}");

        public static DocumentId Parse(string hex)
        {
            return new DocumentId(Convert.ToUInt32(hex, 16));
        }

        public static bool operator ==(DocumentId lhs, DocumentId rhs)
        {
            return lhs.Id == rhs.Id;
        }

        public static bool operator !=(DocumentId lhs, DocumentId rhs)
        {
            return lhs.Id != rhs.Id;
        }

        public override bool Equals(object obj)
        {
            return obj is DocumentId id &&
                   Id == id.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }

        public static ushort ParseBlockId(string hex) => Convert.ToUInt16(hex, 16);

        public static ushort ParseLocalId(string hex) => Convert.ToUInt16(hex, 16);
    }
}
