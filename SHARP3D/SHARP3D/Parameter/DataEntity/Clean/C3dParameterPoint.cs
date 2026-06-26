namespace SHARP3D.Parameter.DataEntity.Clean
{
    public  class C3dParameterPoint
    {
        public float Rate = 0;
        //public float Scale = 0; We don't need Scale. It is calculated when saving the file to C3D. Won't be of any use for our file.
        public string Units = "mm";
    }

}