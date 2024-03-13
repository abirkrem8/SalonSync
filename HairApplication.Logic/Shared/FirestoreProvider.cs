﻿using Google.Cloud.Firestore;
using HairApplication.Models.Entities;

namespace HairApplication.Logic.Shared
{

    public class FirestoreProvider
    {
        private readonly FirestoreDb _fireStoreDb = null!;

        public FirestoreProvider(FirestoreDb fireStoreDb)
        {
            _fireStoreDb = fireStoreDb;
        }

        public async Task AddOrUpdate<T>(T entity, CancellationToken ct) where T : IFirebaseEntity
        {
            var document = _fireStoreDb.Collection($"{typeof(T).Name}s").Document(entity.Id);
            await document.SetAsync(entity, cancellationToken: ct);
        }

        public async Task<T> Get<T>(string id, CancellationToken ct) where T : IFirebaseEntity
        {
            var document = _fireStoreDb.Collection($"{typeof(T).Name}s").Document(id);
            var snapshot = await document.GetSnapshotAsync(ct);
            return snapshot.ConvertTo<T>();
        }

        public async Task<IReadOnlyCollection<T>> GetAll<T>(CancellationToken ct) where T : IFirebaseEntity
        {
            var collection = _fireStoreDb.Collection($"{typeof(T).Name}s");
            var snapshot = await collection.GetSnapshotAsync(ct);

            var results = new List<T>();

            foreach (var document in snapshot.Documents)
            {
                T entity = document.ConvertTo<T>();
                entity.Id = document.Id;
                results.Add(entity);
            }
            return results;
        }

        public async Task<IReadOnlyCollection<T>> WhereEqualTo<T>(string fieldPath, object value, CancellationToken ct) where T : IFirebaseEntity
        {
            return await GetList<T>(_fireStoreDb.Collection($"{typeof(T).Name}s").WhereEqualTo(fieldPath, value), ct);
        }

        // just add here any method you need here WhereGreaterThan, WhereIn etc ...

        private static async Task<IReadOnlyCollection<T>> GetList<T>(Query query, CancellationToken ct) where T : IFirebaseEntity
        {
            var snapshot = await query.GetSnapshotAsync(ct);
            return snapshot.Documents.Select(x => x.ConvertTo<T>()).ToList();
        }
    }

}