public class FirestoreService : IFirestoreService
{
    private readonly FirestoreDb _firestoreDb;
    private const string _collectionName = "Shoes";

    public FirestoreService(FirestoreDb firestoreDb)
    {
        _firestoreDb = firestoreDb;
    }

    public async Task<List<Shoe>> GetAll()
    {
        var collection = _firestoreDb.Collection(_collectionName);
        var snapshot = await collection.GetSnapshotAsync();

        var shoeDocuments = snapshot.Documents.Select(s => s.ConvertTo<ShoeDocument>()).ToList();

        return shoeDocuments.Select(ConvertDocumentToModel).ToList();
    }

    public async Task AddAsync(Shoe shoe)
    {
        var collection = _firestoreDb.Collection(_collectionName);
        var shoeDocument = ConvertModelToDocument(shoe);

        await collection.AddAsync(shoeDocument);
    }

    private static Shoe ConvertDocumentToModel(ShoeDocument shoeDocument)
    {
        return new Shoe
        {
            Id = shoeDocument.Id,
            Name = shoeDocument.Name,
            Brand = shoeDocument.Brand,
            Price = decimal.Parse(shoeDocument.Price)
        };
    }

    private static ShoeDocument ConvertModelToDocument(Shoe shoe)
    {
        return new ShoeDocument
        {
            Id = shoe.Id,
            Name = shoe.Name,
            Brand = shoe.Brand,
            Price = shoe.Price.ToString()
        };
    }
}