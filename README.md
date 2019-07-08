# Information Retrieval

[![Build Status](https://dev.azure.com/aligusnet/InformationRetrieval/_apis/build/status/aligusnet.InformationRetrieval?branchName=master)](https://dev.azure.com/aligusnet/InformationRetrieval/_build/latest?definitionId=3&branchName=master)

## Definitions

List of definitions of key types and concepts.

### Corpus

The project defines the way how the collection of documents is organized in the _corpus_ and _blocks_.

* __Corpus__ - collection of text documents organized into _blocks_;
* __Block__ - subset of corpus, small enough to fit processing in memory;
* __Document__ - piece of text with metadata, the most important metadata is _DocumentId_;
* __DocumentId__ - unique identifier of the document. _DocumentId_ consists of 2 IDs: 
  * _BlockId_ is a unique identifier of the document's block and
  * _LocalId_ is a unique identifier inside the _block_.

### InformationRetrieval

The projects defines a number of types to process text documents organized in corpus.

* __Tranformer__ - converts a corpus of documents, preserving the structure of the corpus, but changing the presentation: texts parsing/cleaning/tokenization etc.
* __Indexer__ - builds an index from a corpus.
* __Token__ is a tuple of term, document id and term's position in the document.
* __BuildableIndex__ is a type used to build an index out of list of _tokens_, created _SearchableIndex__.
* __SearchableIndex__ supports search for a term in the corpus.
* __Boolean Search Engine__ - performs text serching in the corpus using the index. Supports AND/OR/NOT operators.

### Wikidump

A set of types to build a _corpus_ from a [Wikipedia's dump](https://dumps.wikimedia.org/).
