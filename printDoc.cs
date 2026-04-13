class printDoc
{
    private string str;
    private int senderID;

    public void setStr(string s, int id)
    {
        str = s;
        senderID = id;
    }

    public string getStr()
    {
        return str;
    }

    public int getSender()
    {
        return senderID;
    }

    public printDoc(string s, int id)
    {
        str = s;
        senderID = id;
    }
}