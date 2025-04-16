using System;
using System.Collections.Generic;
using System.Drawing;
using System.Media;
using System.Windows.Forms;

namespace gtfoSelector
{
    public partial class form1 : Form
    {
        private List<string> mapList = new List<string>
    {
        "R1A1 : THE ADMIN", "R1B1 : PID SEARCH", "R1B2 : THE OFFICER", "R1C1 : RECONNECT", "R1C2 : DECODE", "R1D1 : DEEPER",
        "R2A1 : THE DIG", "R2B1 : SACRIFICE", "R2B3 : PATHFINDER", "R2B4 : SEPTIC", "R2C1 : TRIANGULATION", "R2C2 : ???", "R2D1 : STATISTICS", "R2D2 : POWERLESS", "R2E1 : CRIB",
        "R3A1 : RESUSCITATION", "R3A2 : PURIFICATION", "R3A3 : BOLT", "R3B1 : THRESHOLD", "R3B2 : PRESSURE", "R3C1 : ALPHA", "R3D1 : BIANHUA",
        "R4A1 : CYTOLOGY", "R4A2 : FOSTER", "R4A3 : ONWARDS", "R4B1 : MALACHITE", "R4B2 : VIRISCITE", "R4B3 : CHRYSOLITE", "R4C1 : CONGNITION", "R4C2 : PABULUM", "R4C3 : CUERNOS", "R4D1 : NUCLEUS", "R4D2 : GROWTH", "R4E1 : DOWNWARDS",
        "R5A1 : FLOODWAYS", "R5A2 : RECOLLECT", "R5A3 : MINING", "R5B1 : SMOTHER", "R5B2 : DISCHARGE", "R5B3 : UNSEAL", "R5B4 : DIVERSION", "R5C1 : BINARY", "R5C2 : ACCESS", "R5C3 : STARVATION", "R5D1 : EVEN DEEPER", "R5D2 : ERROR", "R5E1 : KDS DEEP",
        "R6A1 : ARTIFACT 7", "R6B1 : HEXAHEDRONS", "R6B2 : CONTAMINANT", "R6C1 : NAVIGATION", "R6C2 : BLIND", "R6C3 : PRESSURE POINT", "R6D1 : NEMESIS", "R6D2 : CROSSWAYS", "R6D3 : POWER HUNGRY", "R6D4 : CRYPTOMNESIA", "R6AX : DUST", "R6BX : FLUX", "R6CX : ASCENT",
        "R7A1 : UNIT 23", "R7B1 : CARGO", "R7B2 : DENSE", "R7B3 : VAULT", "R7C1 : MONSTER", "R7C2 : SUBLIMATION", "R7C3 : RECKLESS", "R7D1 : MOTHER", "R7D2 : AWOL", "R7E1 : CHAOS",
        "R8A1 : ORDER", "R8B1 : EFFECT", "R8B2 : WARP", "R8C1 : LINK", "R8D1 : TANDEM", "R8E1 : VALIANT", "R8A2 : PERSPECTIVE", "R8B3 : CAUSE", "R8B4 : INSURRECTION", "R8C2 : UNPLUGGED", "R8D2 : SLAUGHTER", "R8E2 : RELEASE"
        
        };

        private Timer rouletteTimer;
        private Timer blinkTimer;
        private Random random = new Random();

        private int currentIndex = 0;
        private int finalIndex = 0;
        private int totalSteps = 0;
        private int stepsTaken = 0;

        private int blinkCount = 0;
        private bool isLabelVisible = true;
        // private Color[] colors = new Color[] { Color.Red, Color.Green, Color.Blue, Color.Yellow, Color.Purple };

        public form1()
        {
            InitializeComponent();

            // 초기 Label 세팅
            labelResult.Text = "";
            labelResult.Font = new Font("Arial", 28, FontStyle.Bold);
            labelResult.TextAlign = ContentAlignment.MiddleCenter;

            // 룰렛 타이머
            rouletteTimer = new Timer();
            rouletteTimer.Interval = 30;
            rouletteTimer.Tick += RouletteTick;

            // 깜빡임 타이머
            blinkTimer = new Timer();
            blinkTimer.Interval = 200;
            blinkTimer.Tick += BlinkTick;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            // 버튼을 다시 눌렀을 때 색상 초기화
            labelResult.ForeColor = Color.Black; // 초기 색상 검정으로 설정
            labelResult.Text = ""; // 초기 텍스트 비워두기
            labelResult.Font = new Font("consolas", 18, FontStyle.Italic);
            labelR1.Text = "";



            currentIndex = random.Next(mapList.Count);
            labelResult.Text = mapList[currentIndex];

            finalIndex = random.Next(mapList.Count);
             
            // 5초 안에 멈추도록 설정
            int totalTimeMs = 5000;
            int intervalStart = 30; // 초기 속도
            int maxSteps = 80; // 대략적인 스텝 수 (속도 조절용)

            // 감속 효과 적용을 위해 정적으로 설정
            totalSteps = maxSteps;
            stepsTaken = 0;

            rouletteTimer.Interval = intervalStart; // 초기 빠른 간격
            rouletteTimer.Start(); // 타이머 시작
        }

        private void RouletteTick(object sender, EventArgs e)
        {
            // 랜덤한 맵 이름을 무작위로 보여줌
            currentIndex = random.Next(mapList.Count);
            labelResult.Text = mapList[currentIndex];


            stepsTaken++;

            // 감속 곡선 적용 (초반 매우 빠르게 → 후반 급격히 느리게)
            double progress = (double)stepsTaken / totalSteps;

            // 더 급격하게 느려지도록 하기 위해 progress에 제곱을 적용
            double adjustedProgress = Math.Pow(progress, 2);  // 진행 정도의 제곱을 사용하여 급격한 감속 적용

            // 속도가 빨라지다가 점점 급격하게 느려지도록 설정
            rouletteTimer.Interval = 30 + (int)(adjustedProgress * 170); // 30~200ms

            // 5초가 지나면 멈추기
            if (stepsTaken >= totalSteps)
            {
                rouletteTimer.Stop();
                PlaySound();

                // 색을 확실히 보여주고 깜빡이기 시작
                labelResult.ForeColor = Color.Yellow; // 최종 색을 블랙으로 설정

                blinkCount = 0;
                isLabelVisible = true;
                blinkTimer.Start();

                if (mapList[currentIndex].StartsWith("R1"))
                {
                    labelR1.Text = "Rundown 1을 ..?";
                    MessageBox.Show("  R1을 간다?", "R1", MessageBoxButtons.OK);
                }
            }

        }




        private void BlinkTick(object sender, EventArgs e)
        {
            isLabelVisible = !isLabelVisible;
            labelResult.Visible = isLabelVisible;
            blinkCount++;

            if (blinkCount >= 10)
            {
                blinkTimer.Stop();
                labelResult.Visible = true;
            }
        }

        private void PlaySound()
        {
            try
            {
                // 현재 없음
                SoundPlayer player = new SoundPlayer("done.wav");
                player.Play();
            } catch
            {
                SystemSounds.Asterisk.Play();
                // 파일 없을 경우 무시
            }
        }
    }

}
