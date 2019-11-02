using System;
using System.Collections.Generic;
using System.Text;

namespace raBudget.Domain.Entities
{
    public class BaseEntity<T> : IEquatable<T>
    {
        public T Id { get; set; }

        #region Equality members

        protected bool Equals(BaseEntity<T> other)
        {
            return EqualityComparer<T>.Default.Equals(Id, other.Id);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != this.GetType())
                return false;
            return Equals((BaseEntity<T>) obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return EqualityComparer<T>.Default.GetHashCode(Id);
        }

        public bool Equals(T other)
        {
            return Id.Equals(other);
        }

        #endregion
    }
}