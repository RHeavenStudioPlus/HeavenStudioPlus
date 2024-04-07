public struct NoteSample
{
    public int note;
    public int octave;
    public string sample;
    
    public NoteSample(string sample, int note, int octave)
    {
        this.note = note;
        this.octave = octave;
        this.sample = sample;
    }
}
