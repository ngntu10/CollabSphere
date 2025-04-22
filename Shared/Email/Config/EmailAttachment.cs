namespace CollabSphere.Modules.Email.Config;

public class EmailAttachment
{
    public byte[] Value { get; private set; }

    public string Name { get; private set; }

    public static EmailAttachment Create(byte[] value, string name)
    {
        return new EmailAttachment
        {
            Value = value,
            Name = name
        };
    }
}
