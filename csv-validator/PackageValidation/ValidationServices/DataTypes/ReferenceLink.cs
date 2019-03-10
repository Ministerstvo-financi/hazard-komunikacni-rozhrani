namespace ValidationPilotServices.DataTypes
{
    public class ReferenceLink : Identifier
    {
        public ReferenceLink() : base() {}
        public ReferenceLink(int minLength, int maxLength) : base(minLength,maxLength) {}
        
    }
}
