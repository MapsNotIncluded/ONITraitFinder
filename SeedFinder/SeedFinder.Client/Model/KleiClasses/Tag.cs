namespace SeedFinder.Client.Model.KleiClasses
{
    using System;
    using System.Linq;

    public struct Tag : IEquatable<Tag>, IComparable<Tag>
    {
        public static readonly Tag Invalid;

        private string name;

        private int hash;

        public static int SDBMLower(string s)
        {
            if (s == null)
            {
                return 0;
            }

            uint num = 0u;
            for (int i = 0; i < s.Length; i++)
            {
                num = char.ToLowerInvariant(s[i]) + (num << 6) + (num << 16) - num;
            }

            return (int)num;
        }



        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = string.Intern(value);
                hash = SDBMLower(name);
            }
        }

        public bool IsValid => hash != 0;

        public Tag(int hash)
        {
            this.hash = hash;
            name = "";
        }

        public Tag(Tag orig)
        {
            name = orig.name;
            hash = orig.hash;
        }

        public Tag(string name)
        {
            this.name = name;
            hash = SDBMLower(name);
        }

        public void Clear()
        {
            name = null;
            hash = 0;
        }

        public override int GetHashCode()
        {
            return hash;
        }

        public int GetHash()
        {
            return hash;
        }

        public override bool Equals(object obj)
        {
            Tag tag = (Tag)obj;
            return hash == tag.hash;
        }

        public bool Equals(Tag other)
        {
            return hash == other.hash;
        }

        public static bool operator ==(Tag a, Tag b)
        {
            return a.hash == b.hash;
        }

        public static bool operator !=(Tag a, Tag b)
        {
            return a.hash != b.hash;
        }

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            if (name != null)
            {
                Name = name;
            }
            else
            {
                name = "";
            }
        }

        public int CompareTo(Tag other)
        {
            return hash - other.hash;
        }

        public override string ToString()
        {
            if (name == null)
            {
                return hash.ToString("X");
            }

            return name;
        }

        public static implicit operator Tag(string s)
        {
            return new Tag(s);
        }

        public static string ArrayToString(Tag[] tags)
        {
            return string.Join(",", tags.Select((Tag x) => x.ToString()).ToArray());
        }
    }
}
