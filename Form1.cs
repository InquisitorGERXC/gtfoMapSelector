using System;
using System.Collections.Generic;
using System.Drawing;
using System.Media;
using System.Windows.Forms;

namespace gtfoSelector
{
    public partial class form1 : Form
    {
        private List<string> mapList = new List<string> // 맵 리스트 초기화 (룰렛 표시)
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

        // 룰렛에 사용될 필터링 된 맵의 리스트 초기화
        private List<string> rotatingList;

        // 룰렛과 관련된 타이머 오브젝트
        private Timer rouletteTimer;
        private Timer blinkTimer;

        // 난수 생성자
        private Random random = new Random();

        // 현재 인덱스, 단계 수, 총 단계수 등.. 각종 변수 초기화
        private int currentIndex = 0;
        private int stepsTaken = 0;
        private int totalSteps = 0;
        private int blinkCount = 0;
        private bool isCenterVisible = true;


        // 화면 폼 Constructor
        public form1()
        {
            InitializeComponent(); 

            // Setup roulette timer
            rouletteTimer = new Timer();
            rouletteTimer.Interval = 30;
            rouletteTimer.Tick += RouletteTick;

            // Setup blink timer
            blinkTimer = new Timer();
            blinkTimer.Interval = 200;
            blinkTimer.Tick += BlinkTick;

            // Label 초기화 (should exist in the designer)
            LabelTop.Text = "R1A1 : THE ADMIN";
            LabelCenter.Text = "R1B1 : PID SEARCH";
            LabelBottom.Text = "R1B2 : THE OFFICER";
        } // form1()

        
        private void btnStart_Click(object sender, EventArgs e)
        {
            labelR1.Text = "";
            stepsTaken = 0;
            totalSteps = 90; // Number of steps for the spin
            LabelCenter.ForeColor = Color.Black;

            // Validate speed input
            if (string.IsNullOrWhiteSpace(txtSpeed.Text))
            {
                MessageBox.Show("속도를 입력해주세요.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            } // if

            if (!int.TryParse(txtSpeed.Text, out int speed) || speed < 1 || speed > 100)
            {
                MessageBox.Show("1부터 100 사이의 숫자를 입력해주세요.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            } // if

            // 속도 → interval 계산 (작을수록 빠르게, 클수록 느리게)
            // 예: 최대 속도 = interval 10, 최소 속도 = interval 100
            int interval = 110 - speed; // 100 입력 시 interval = 10, 1 입력 시 interval = 109
            rouletteTimer.Interval = interval;
            // 선형 방식이라 어차피 체감 매우 적음
            // 사람은 지수 변화를 변화해야 빠르거나 느려지는 것을 판단
            // 코드 작성은 매우 복잡해지고 꼬일 위험이 있다는 것을 고려해서 지수 함수를 적용하면 오류가 발생 할 가능성이 있음
            // = 개발 역량 부족

            // 필터링된 리스트 사용
            List<string> filteredMapList = GetFilteredMapList();

            if (filteredMapList.Count < 3)
            {
                MessageBox.Show("룰렛을 돌리기 위해 최소 3개 이상의 맵이 필요합니다.");
                return;
            } // if

            rotatingList = filteredMapList;
            currentIndex = random.Next(rotatingList.Count); // 꼭 갱신

            rouletteTimer.Start(); // Start spinning

        } // btnStart_Click()


        // Restrict txtSpeed input to numbers only
        private void txtSpeed_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            } // if
        } // txtSpeed_KeyPress()


        // Called on each tick of the roulette timer
        private void RouletteTick(object sender, EventArgs e)
        {
            // 배열 순환 방식으로 인덱스 증가
            if (rotatingList == null || rotatingList.Count < 3)
            {
                rouletteTimer.Stop();
                MessageBox.Show("룰렛 항목이 부족합니다.");
                return;
            } // if

            // Move to next item in a circular way
            currentIndex = (currentIndex + 1) % rotatingList.Count;

            // Determine the items to display in the top, center, and bottom
            string top = rotatingList[(currentIndex - 1 + rotatingList.Count) % rotatingList.Count];
            string center = rotatingList[currentIndex];
            string bottom = rotatingList[(currentIndex + 1) % rotatingList.Count];

            // Label 초기화
            LabelTop.Text = top;
            LabelCenter.Text = center;
            LabelBottom.Text = bottom;

            stepsTaken++;

            // Adjust speed to simulate deceleration
            double progress = (double)stepsTaken / totalSteps;
            double adjusted = Math.Pow(progress, 2);
            rouletteTimer.Interval = 30 + (int)(adjusted * 170);

            // If done spinning
            if (stepsTaken >= totalSteps)
            {
                rouletteTimer.Stop();
                PlaySound(); // Play completion sound

                LabelCenter.ForeColor = Color.Yellow;
                blinkCount = 0;
                isCenterVisible = true;
                blinkTimer.Start(); // Start blinking effect

                // Special message for R1 result
                if (center.StartsWith("R1"))
                {
                    labelR1.Text = "Rundown 1을 ..?";
                    MessageBox.Show("  R1을 간다?", "R1", MessageBoxButtons.OK);
                } // inner-if

            } // if

        } // RouletteTick()

        // Blinking effect for selected result
        private void BlinkTick(object sender, EventArgs e)
        {
            isCenterVisible = !isCenterVisible;
            LabelCenter.Visible = isCenterVisible;
            blinkCount++;

            if (blinkCount >= 10)
            {
                blinkTimer.Stop();
                LabelCenter.Visible = true;
            }
        }

        // Play sound effect when roulette ends
        private void PlaySound() // 현재 기능 추가 X
        {
            try
            {
                SoundPlayer player = new SoundPlayer("done.wav");
                player.Play();
            } catch
            {
                // Use system sound as fallback
                SystemSounds.Asterisk.Play();
            }
        } // PlaySound()

        // Filter the full map list based on user checkboxes
        private List<string> GetFilteredMapList()
        {
            List<string> filtered = new List<string>();

            foreach (string map in mapList)
            {
                if ((chkR1.Checked && map.StartsWith("R1")) ||
                    (chkR2.Checked && map.StartsWith("R2")) ||
                    (chkR3.Checked && map.StartsWith("R3")) ||
                    (chkR4.Checked && map.StartsWith("R4")) ||
                    (chkR5.Checked && map.StartsWith("R5")) ||
                    (chkR6.Checked && map.StartsWith("R6")) ||
                    (chkR7.Checked && map.StartsWith("R7")) ||
                    (chkR8.Checked && map.StartsWith("R8")) ||
                    (chkA.Checked && map.Contains("A")) ||
                    (chkB.Checked && map.Contains("B")) ||
                    (chkC.Checked && map.Contains("C")) ||
                    (chkD.Checked && map.Contains("D")) ||
                    (chkE.Checked && map.Contains("E")))

                {
                    continue; // 체크된 것은 제외
                } // if

                filtered.Add(map); // Add map to result
            } // foreach

            return filtered;
        } // GetFilteredMapList()

    } // end class
}
