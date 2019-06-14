using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace User.API.Models
{
    public class UserProperty
    {
        int? _requestedHashCode;
        public Guid AppUserId { get; set; }
        public string Key { get; set; }
        public string Text { get; set; }
        public string Value { get; set; }

        public override int GetHashCode()
        {
            if (!IsTransient())
            {
                if (!_requestedHashCode.HasValue)
                {
                    _requestedHashCode = (this.Key + this.Value).GetHashCode() ^ 31;// XOP for random distribution(http://blogs.msdn.com/b/ericlippert/archive)
                }
            }
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is UserProperty))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            UserProperty item = (UserProperty)obj;
            if (item.IsTransient() || IsTransient())
            {
                return false;
            }
            else
            {
                return item.Key == this.Key && item.Value == this.Value;
            }
        }
        public bool IsTransient()
        {
            return string.IsNullOrWhiteSpace(this.Key) || string.IsNullOrWhiteSpace(Value);
        }

        public static bool operator ==(UserProperty left, UserProperty right)
        {
            if (Equals(left, null))
            {
                return (object.Equals(right, null)) ? true : false;
            }
            else
            {
                return left.Equals(right);
            }
        }
        public static bool operator !=(UserProperty left, UserProperty right)
        {
            return !(left == right);
        }

    }
}
