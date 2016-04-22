﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Jasily.ComponentModel.Editable
{
    internal static class JasilyEditableViewModel
    {
        internal interface IEditableViewModel
        {
            void WriteToObject(object obj);

            void ReadFromObject(object obj);
        }

        private abstract class Executor
        {
            protected Executor(string name, EditableFieldAttribute attribute)
            {
                Debug.Assert(name != null);
                Debug.Assert(attribute != null);
                this.Name = name;
                this.Attribute = attribute;
            }

            public string Name { get; }

            public EditableFieldAttribute Attribute { get; }

            public Getter<object, object> ViewModelGetter { get; set; }

            public virtual void Verify()
            {
                Debug.Assert(this.ViewModelGetter != null);
            }

            public abstract void WriteToObject(object vm, object obj);

            public abstract void ReadFromObject(object obj, object vm);
        }

        private class SubViewModelCaller : Executor
        {
            public override void WriteToObject(object vm, object obj)
            {
                Debug.Assert(vm != null && obj != null);
                var value = this.ViewModelGetter.Get(vm);
                if (value == null) return;
                Debug.Assert(value is IEditableViewModel);
                ((IEditableViewModel)value).WriteToObject(obj);
            }

            public override void ReadFromObject(object obj, object vm)
            {
                Debug.Assert(vm != null && obj != null);
                var value = this.ViewModelGetter.Get(vm);
                if (value == null) return;
                Debug.Assert(value is IEditableViewModel);
                ((IEditableViewModel)value).ReadFromObject(obj);
            }

            public SubViewModelCaller(string name, EditableFieldAttribute attribute)
                : base(name, attribute)
            {
            }
        }

        private class FieldWriter : Executor
        {
            public FieldWriter(string name, EditableFieldAttribute attribute)
                : base(name, attribute)
            {
            }

            public Setter<object, object> ViewModelSetter { get; set; }

            public Getter<object, object> ObjectGetter { get; set; }

            public Setter<object, object> ObjectSetter { get; set; }

            public override void Verify()
            {
                base.Verify();

                Debug.Assert(this.ViewModelSetter != null);
                Debug.Assert(this.ObjectGetter != null);
                Debug.Assert(this.ObjectSetter != null);

                if (this.Attribute.Converter != null)
                {
                    if (!typeof(IConverter).GetTypeInfo().IsAssignableFrom(this.Attribute.Converter.GetTypeInfo()))
                        throw new InvalidCastException($"can not cast {this.Attribute.Converter} to {typeof(IConverter)}");

                    if (this.Attribute.Converter.GetTypeInfo()
                        .DeclaredConstructors
                        .FirstOrDefault(z => z.GetParameters().Length == 0) == null)
                        throw new InvalidOperationException($"{this.Attribute.Converter} has no none args constructor.");
                }
            }

            public override void WriteToObject(object vm, object obj)
            {
                Debug.Assert(vm != null && obj != null);
                var value = this.ViewModelGetter.Get(vm);
                if (this.Attribute.Converter != null)
                {
                    var converter = Activator.CreateInstance(this.Attribute.Converter) as IConverter;
                    Debug.Assert(converter != null);
                    if (!converter.CanConvertBack(value)) return;
                    value = converter.ConvertBack(value);
                }
                this.ObjectSetter.Set(obj, value);
            }

            public override void ReadFromObject(object obj, object vm)
            {
                Debug.Assert(vm != null && obj != null);
                var value = this.ObjectGetter.Get(obj);
                if (this.Attribute.Converter != null)
                {
                    var converter = Activator.CreateInstance(this.Attribute.Converter) as IConverter;
                    Debug.Assert(converter != null);
                    if (!converter.CanConvert(value)) return;
                    value = converter.Convert(value);
                }
                this.ViewModelSetter.Set(vm, value);
            }
        }

        public class Cache
        {
            private Dictionary<string, Executor> executors;

            protected Type ViewModelType { get; }

            protected Type ObjectType { get; }

            public Cache(Type viewModelType, Type objectType)
            {
                this.ViewModelType = viewModelType;
                this.ObjectType = objectType;

                this.MappingType();
            }

            private void MappingType()
            {
                if (this.executors == null)
                {
                    var mapped = new Dictionary<string, Executor>();

                    // view model
                    foreach (var field in this.ViewModelType.GetRuntimeFields())
                    {
                        var attr = field.GetCustomAttribute<EditableFieldAttribute>();
                        if (attr != null)
                        {
                            if (attr.IsSubEditableViewModel)
                            {
                                var executor = new SubViewModelCaller(field.Name, attr);
                                executor.ViewModelGetter = field.CompileGetter();
                                mapped.Add(field.Name, executor);
                            }
                            else
                            {
                                var executor = new FieldWriter(field.Name, attr);
                                executor.ViewModelGetter = field.CompileGetter();
                                executor.ViewModelSetter = field.CompileSetter();
                                mapped.Add(field.Name, executor);
                            }
                        }
                    }
                    foreach (var property in this.ViewModelType.GetRuntimeProperties())
                    {
                        var attr = property.GetCustomAttribute<EditableFieldAttribute>();
                        if (attr != null)
                        {
                            if (attr.IsSubEditableViewModel)
                            {
                                var executor = new SubViewModelCaller(property.Name, attr);
                                executor.ViewModelGetter = property.CompileGetter();
                                mapped.Add(property.Name, executor);
                            }
                            else
                            {
                                var writer = new FieldWriter(property.Name, attr);
                                writer.ViewModelGetter = property.CompileGetter();
                                writer.ViewModelSetter = property.CompileSetter();
                                mapped.Add(property.Name, writer);
                            }
                        }
                    }

                    // object
                    foreach (var field in this.ObjectType.GetRuntimeFields())
                    {
                        Executor executor;
                        if (mapped.TryGetValue(field.Name, out executor))
                        {
                            var writer = executor as FieldWriter;
                            if (writer != null)
                            {
                                writer.ObjectGetter = field.CompileGetter();
                                writer.ObjectSetter = field.CompileSetter();
                            }
                        }
                    }
                    foreach (var property in this.ObjectType.GetRuntimeProperties())
                    {
                        Executor executor;
                        if (mapped.TryGetValue(property.Name, out executor))
                        {
                            var writer = executor as FieldWriter;
                            if (writer != null)
                            {
                                writer.ObjectGetter = property.CompileGetter();
                                writer.ObjectSetter = property.CompileSetter();
                            }
                        }
                    }

                    mapped.Values.ForEach(z => z.Verify());
                    this.executors = mapped;
                }
            }

            public void WriteToObject(object vm, object obj)
            {
                Debug.Assert(vm != null);
                Debug.Assert(obj != null);
                Debug.Assert(vm.GetType() == this.ViewModelType);
                Debug.Assert(obj.GetType() == this.ObjectType);

                foreach (var writer in this.executors.Values)
                {
                    writer.WriteToObject(vm, obj);
                }
            }

            public void ReadFromObject(object obj, object vm)
            {
                Debug.Assert(vm != null);
                Debug.Assert(obj != null);
                Debug.Assert(vm.GetType() == this.ViewModelType);
                Debug.Assert(obj.GetType() == this.ObjectType);

                foreach (var writer in this.executors.Values)
                {
                    writer.ReadFromObject(obj, vm);
                }
            }
        }

        public class Cache<T>
        {
            // ReSharper disable once StaticMemberInGenericType
            private static readonly Dictionary<Type, Cache> Cached = new Dictionary<Type, Cache>();

            public static Cache GetMapperCache(Type viewModelType)
            {
                lock (Cached)
                {
                    var ret = Cached.GetValueOrDefault(viewModelType);
                    if (ret != null) return ret;
                }

                var @new = new Cache(viewModelType, typeof(T));

                lock (Cached)
                {
                    return Cached.GetOrSetValue(viewModelType, @new);
                }
            }
        }
    }

    public abstract class JasilyEditableViewModel<T> : JasilyViewModel<T>,
        JasilyEditableViewModel.IEditableViewModel
    {
        private JasilyEditableViewModel.Cache mapperCached;

        protected JasilyEditableViewModel(T source)
            : base(source)
        {
        }

        public virtual void WriteToObject(T obj)
        {
            if (ReferenceEquals(obj, null)) return;
            if (this.mapperCached == null)
                this.mapperCached = JasilyEditableViewModel.Cache<T>.GetMapperCache(this.GetType());
            this.mapperCached.WriteToObject(this, obj);
        }

        public virtual void ReadFromObject(T obj)
        {
            if (ReferenceEquals(obj, null)) return;
            if (this.mapperCached == null)
                this.mapperCached = JasilyEditableViewModel.Cache<T>.GetMapperCache(this.GetType());
            this.mapperCached.ReadFromObject(obj, this);
        }

        #region Implementation of IEditableViewModel

        void JasilyEditableViewModel.IEditableViewModel.WriteToObject(object obj)
        {
            Debug.Assert(obj is T);
            if (this.mapperCached == null)
                this.mapperCached = JasilyEditableViewModel.Cache<T>.GetMapperCache(this.GetType());
            this.mapperCached.WriteToObject(this, obj);
        }

        void JasilyEditableViewModel.IEditableViewModel.ReadFromObject(object obj)
        {
            Debug.Assert(obj is T);
            if (this.mapperCached == null)
                this.mapperCached = JasilyEditableViewModel.Cache<T>.GetMapperCache(this.GetType());
            this.mapperCached.ReadFromObject(obj, this);
        }

        #endregion
    }
}