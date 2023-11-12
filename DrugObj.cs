namespace Azure.Function{
 
    public class Drug{
 
        //Overloading Method
        public Drug(string name, bool available, string brand){
            this.Name = name;
            this.Available = available;
            this.Brand = brand;
        }
 
 
        public string Name {get;set;}
        public bool Available {get;set;}
        public string Brand {get;set;}
 
    }
 
 
 
}