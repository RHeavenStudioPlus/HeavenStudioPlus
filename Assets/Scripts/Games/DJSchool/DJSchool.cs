using RhythmHeavenMania.Util;

namespace RhythmHeavenMania.Games.DJSchool
{
    public class DJSchool : Minigame
    {
        public static DJSchool instance { get; private set; }

        private void Awake()
        {
            instance = this;
        }

        public void BreakCmon(float beat)
        {
            MultiSound.Play(new MultiSound.Sound[]
            {
                new MultiSound.Sound("djSchool/breakCmon1",   beat),
                new MultiSound.Sound("djSchool/breakCmon2", beat + 1f),
                new MultiSound.Sound("djSchool/ooh", beat + 2f),
            });
        }

        public void ScratchoHey(float beat)
        {
            MultiSound.Play(new MultiSound.Sound[]
            {
                new MultiSound.Sound("djSchool/scratchoHey1",   beat),
                new MultiSound.Sound("djSchool/scratchoHey2", beat + 1f),
                new MultiSound.Sound("djSchool/hey", beat + 2f),
            });
        }
    }
}