﻿using System.Diagnostics.CodeAnalysis;

namespace Do.Domain.Model;

public class ModelCollection<T> : IEnumerable<T>, IModelCollectionWithIndex<T>
    where T : IModel
{
    readonly ModelKeyedCollection<T> _models = [];
    readonly Dictionary<ModelIndexKey, ModelCollection<T>> _index = [];

    public ModelCollection() { }

    public ModelCollection(IEnumerable<T> models)
    {
        foreach (var model in models)
        {
            _models.Add(model);
        }
    }

    public T this[string id] =>
        _models[id];

    public T this[Type type] =>
        _models[TypeModelReference.IdFrom(type)];

    public int Count => _models.Count;

    public bool ContainsModel(T? model) =>
        _models.Contains(model?.Id ?? string.Empty);

    public bool Contains(string id) =>
        _models.Contains(id);

    public bool TryGetValue(string id, [NotNullWhen(true)] out T? model) =>
       _models.TryGetValue(id, out model);

    public ModelCollection<T> GetIndex(object key) =>
        _index.TryGetValue(new(key), out var result) ? result : new([]);

    public ModelCollection<T> Having<TAttribute>() where TAttribute : Attribute =>
        GetIndex(typeof(TAttribute));
    public ModelCollection<T> Having<TAttribute>(TAttribute attribute) where TAttribute : Attribute =>
        GetIndex(attribute);

    public IEnumerator<T> GetEnumerator() => _models.GetEnumerator();

    Dictionary<ModelIndexKey, ModelCollection<T>> IModelCollectionWithIndex<T>.Index => _index;
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
