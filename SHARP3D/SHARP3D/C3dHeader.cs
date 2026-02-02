struct C3dHeader { 
    public byte pointerParameterSection;
    public byte flagDataFormat;
    public int markersPerFrame;
    public int analogSamplesPerFrame;
    public int firstFrameRawData;
    public int lastFrameRawData;
    public int maxFrameIntepolationGap;
    public int ScaleFactor;
    public byte pointerDataSection;
    public int analogSampleRatePerFrame;
    public float rate3dFrame;
    public bool support4charEventLabels;
    public int nbDefinedEvents;
    public C3dHeaderEvent[] events;
}