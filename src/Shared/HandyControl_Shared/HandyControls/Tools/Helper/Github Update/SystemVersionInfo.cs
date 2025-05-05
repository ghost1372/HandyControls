using System;

namespace HandyControl.Tools;

public readonly struct SystemVersionInfo
{
    public SystemVersionInfo(int major, int minor, int build, int revision = 0)
    {
        Major = major;
        Minor = minor;
        Build = build;
        Revision = revision;
    }

    public int Major { get; }

    public int Minor { get; }

    public int Build { get; }

    public int Revision { get; }

    public bool Equals(SystemVersionInfo other)
    {
        return Major == other.Major && Minor == other.Minor && Build == other.Build && Revision == other.Revision;
    }

    public override bool Equals(object obj)
    {
        return obj is SystemVersionInfo other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Major.GetHashCode() ^ Minor.GetHashCode() ^ Build.GetHashCode() ^ Revision.GetHashCode();
    }

    public static bool operator ==(SystemVersionInfo left, SystemVersionInfo right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(SystemVersionInfo left, SystemVersionInfo right)
    {
        return !(left == right);
    }

    public int CompareTo(SystemVersionInfo other)
    {
        return Major != other.Major
            ? Major.CompareTo(other.Major)
            : Minor != other.Minor
            ? Minor.CompareTo(other.Minor)
            : Build != other.Build ? Build.CompareTo(other.Build) : Revision != other.Revision ? Revision.CompareTo(other.Revision) : 0;
    }

    public int CompareTo(object obj)
    {
        return obj is not SystemVersionInfo other ? throw new ArgumentException() : CompareTo(other);
    }

    public static bool operator <(SystemVersionInfo left, SystemVersionInfo right)
    {
        return left.CompareTo(right) < 0;
    }

    public static bool operator <=(SystemVersionInfo left, SystemVersionInfo right)
    {
        return left.CompareTo(right) <= 0;
    }

    public static bool operator >(SystemVersionInfo left, SystemVersionInfo right)
    {
        return left.CompareTo(right) > 0;
    }

    public static bool operator >=(SystemVersionInfo left, SystemVersionInfo right)
    {
        return left.CompareTo(right) >= 0;
    }

    public override string ToString()
    {
        return $"{Major}.{Minor}.{Build}.{Revision}";
    }
}
