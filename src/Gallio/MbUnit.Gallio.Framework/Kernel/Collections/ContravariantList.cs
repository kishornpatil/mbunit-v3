using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace MbUnit.Framework.Kernel.Collections
{
    /// <summary>
    /// A contravariant list converts a list of the input type to a
    /// read-only list of the more generic output type.
    /// </summary>
    /// <typeparam name="TInput">The input list type</typeparam>
    /// <typeparam name="TOutput">The output list type</typeparam>
    public class ContravariantList<TInput, TOutput> : IList<TOutput>
        where TInput : TOutput
    {
        private readonly IList<TInput> inputList;

        /// <summary>
        /// Creates a wrapper for the specified list.
        /// </summary>
        /// <param name="inputList">The input list</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="inputList"/> is null</exception>
        public ContravariantList(IList<TInput> inputList)
        {
            if (inputList == null)
                throw new ArgumentNullException(@"inputList");

            this.inputList = inputList;
        }

        /// <inheritdoc />
        public int Count
        {
            get { return inputList.Count; }
        }

        /// <inheritdoc />
        public TOutput this[int index]
        {
            get { return inputList[index]; }
        }

        /// <inheritdoc />
        public bool IsReadOnly
        {
            get { return true; }
        }

        /// <inheritdoc />
        public void CopyTo(TOutput[] array, int arrayIndex)
        {
            foreach (TInput item in inputList)
                array[arrayIndex++] = item;
        }

        /// <inheritdoc />
        public bool Contains(TOutput item)
        {
            return item is TInput && inputList.Contains((TInput) item);
        }

        /// <inheritdoc />
        public int IndexOf(TOutput item)
        {
            return item is TInput ? inputList.IndexOf((TInput) item) : -1;
        }

        /// <inheritdoc />
        public IEnumerator<TOutput> GetEnumerator()
        {
            foreach (TInput item in inputList)
                yield return item;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        TOutput IList<TOutput>.this[int index]
        {
            get { return this[index]; }
            set { throw new NotSupportedException(); }
        }

        void IList<TOutput>.Insert(int index, TOutput item)
        {
            throw new NotSupportedException();
        }

        void IList<TOutput>.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        void ICollection<TOutput>.Add(TOutput item)
        {
            throw new NotSupportedException();
        }

        void ICollection<TOutput>.Clear()
        {
            throw new NotSupportedException();
        }

        bool ICollection<TOutput>.Remove(TOutput item)
        {
            throw new NotSupportedException();
        }
    }
}
