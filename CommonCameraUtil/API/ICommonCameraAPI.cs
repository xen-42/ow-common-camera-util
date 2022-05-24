namespace CommonCameraUtil.API
{
    public interface ICommonCameraAPI
    {
        void RegisterCustomCamera(OWCamera OWCamera);
        OWCamera CreateCustomCamera(string name);
    }
}
