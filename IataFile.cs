using System;

class IataFile
{
    public string Name { get; set; }
    public string Id { get; set; }
    public DateTime UploadDate { get; set; }
    public long Size { get; set; }
    public IataFileType Type { get; set; }
    public IataFileStatus Status { get; set; }
}
class IataFileType
{
    public string Descriptor { get; set; }
    public string Description { get; set; }
}

enum IataFileStatus
{
    N,
    D
}