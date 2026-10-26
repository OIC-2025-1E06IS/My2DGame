using System.Drawing.Drawing2D;
using System.Media;

namespace My2DGame;

internal sealed class GameForm : Form
{
    enum Mode { Title, Play, Pause, Over }
    Mode mode = Mode.Title;
    readonly System.Windows.Forms.Timer timer = new() { Interval = 16 };
    readonly Random rng = new();
    readonly List<RectangleF> trees = new();
    readonly string savePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NinjaVsMonster", "highscore.txt");
    const float ground = 365, px = 125, pw = 44, ph = 60, mw = 54, mh = 56;
    float py, vy, mx = 850, scroll;
    int score, high, ticks, frame;
    bool counted;
    Rectangle start = new(300, 225, 200, 54), restart = new(300, 265, 200, 52), pause = new(18, 16, 100, 34);

    public GameForm()
    {
        Text = "Ninja vs Monster"; ClientSize = new(800,450); FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false; StartPosition = FormStartPosition.CenterScreen; DoubleBuffered = true; KeyPreview = true;
        py = ground-ph;
        try { if (File.Exists(savePath) && int.TryParse(File.ReadAllText(savePath), out int h)) high=Math.Max(0,h); } catch { }
        for (int i=0;i<12;i++) trees.Add(new(i*95,rng.Next(105,235),rng.Next(42,72),rng.Next(90,145)));
        timer.Tick += TickGame; timer.Start();
    }
    void SaveHigh()
    {
        try { Directory.CreateDirectory(Path.GetDirectoryName(savePath)!); File.WriteAllText(savePath,high.ToString()); } catch { }
    }
    void StartGame() { score=0; mx=850; ticks=0; counted=false; py=ground-ph; vy=0; mode=Mode.Play; Invalidate(); }
    void TickGame(object? s, EventArgs e)
    {
        frame++;
        if(mode==Mode.Play)
        {
            scroll=(scroll+2)%95;
            for(int i=0;i<trees.Count;i++){var t=trees[i];t.X-=1.7f;if(t.Right<0){t.X=800+rng.Next(100);t.Y=rng.Next(105,235);}trees[i]=t;}
            vy+=.58f;py+=vy;if(py>=ground-ph){py=ground-ph;vy=0;}
            ticks++;
            if(mx < -mw){mx=850;ticks=0;counted=false;}
            if(mx>=850 && ticks>=115){mx=800;ticks=0;counted=false;}
            if(mx<850)
            {
                mx-=5+Math.Min(score*.18f,5);
                if(new RectangleF(px+7,py+8,pw-12,ph-8).IntersectsWith(new RectangleF(mx+5,ground-mh+7,mw-10,mh-7))) EndGame();
                else if(!counted && mx+mw<px){score++;counted=true;if(score>high){high=score;SaveHigh();}}
            }
        }
        Invalidate();
    }
    void EndGame(){mode=Mode.Over; if(score>high){high=score;SaveHigh();} SystemSounds.Hand.Play();}
    void Jump(){if(mode==Mode.Play && py>=ground-ph-.5f){vy=-12.5f;SystemSounds.Asterisk.Play();}}
    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);var g=e.Graphics;g.SmoothingMode=SmoothingMode.None;
        using(var sky=new LinearGradientBrush(ClientRectangle,Color.FromArgb(117,204,255),Color.FromArgb(222,247,255),90))g.FillRectangle(sky,ClientRectangle);
        g.FillEllipse(Brushes.Khaki,660,35,55,55);
        foreach(var t in trees){g.FillRectangle(Brushes.DarkSeaGreen,t.X,t.Y+t.Height*.2f,t.Width,t.Height*.6f);g.FillRectangle(Brushes.ForestGreen,t.X+t.Width*.15f,t.Y,t.Width*.7f,t.Height*.65f);g.FillRectangle(Brushes.SaddleBrown,t.X+t.Width*.4f,t.Y+t.Height*.6f,t.Width*.2f,t.Height*.4f);}
        for(int i=-1;i<8;i++){float x=i*125-scroll*1.5f;g.FillRectangle(Brushes.Sienna,x+48,200,22,170);g.FillRectangle(Brushes.ForestGreen,x+12,145,94,92);g.FillRectangle(Brushes.SeaGreen,x+27,115,64,75);}
        g.FillRectangle(Brushes.ForestGreen,0,ground,800,22);g.FillRectangle(Brushes.SaddleBrown,0,ground+22,800,63);
        for(int x=-(int)(scroll*2)%44;x<800;x+=44){g.FillRectangle(Brushes.Peru,x,ground+34,17,5);g.FillRectangle(Brushes.Sienna,x+22,ground+57,10,4);}
        if(mode==Mode.Title)
        {
            Ninja(g,150,ground-ph,true);Monster(g,600,ground-mh);
            Center(g,"NINJA VS MONSTER",new("Consolas",28,FontStyle.Bold),Color.DarkSlateGray,new(30,70,740,60));
            Center(g,"DODGE THE MONSTERS!",new("Consolas",13,FontStyle.Bold),Color.DarkGreen,new(100,130,600,30));
            Button(g,start,"START GAME");Center(g,"SPACE: JUMP     ESC: PAUSE",new("Consolas",11,FontStyle.Bold),Color.DarkSlateGray,new(130,300,540,30));
        }
        else
        {
            Ninja(g,px,py,py>=ground-ph-.5f);if(mx<850)Monster(g,mx,ground-mh);
            g.DrawString($"SCORE: {score}",new("Consolas",14,FontStyle.Bold),Brushes.DarkSlateGray,610,18);
            g.DrawString($"HIGH SCORE: {high}",new("Consolas",9,FontStyle.Bold),Brushes.DarkSlateGray,610,42);
            Button(g,pause,mode==Mode.Pause?"RESUME":"ESC: PAUSE");
            if(mode==Mode.Pause){g.FillRectangle(new SolidBrush(Color.FromArgb(150,20,35,35)),ClientRectangle);Center(g,"PAUSED",new("Consolas",32,FontStyle.Bold),Color.White,new(100,145,600,55));Center(g,"Press ESC to resume",new("Consolas",14,FontStyle.Bold),Color.White,new(100,205,600,35));}
            if(mode==Mode.Over){g.FillRectangle(new SolidBrush(Color.FromArgb(180,17,29,32)),ClientRectangle);Center(g,"GAME OVER",new("Consolas",34,FontStyle.Bold),Color.Salmon,new(100,95,600,55));Center(g,$"SCORE: {score}",new("Consolas",19,FontStyle.Bold),Color.White,new(100,165,600,35));Center(g,$"HIGH SCORE: {high}",new("Consolas",15,FontStyle.Bold),Color.Khaki,new(100,207,600,30));Button(g,restart,"RESTART");Center(g,"Press ENTER to play again",new("Consolas",10,FontStyle.Bold),Color.White,new(150,328,500,25));}
        }
    }
    static void Ninja(Graphics g,float x,float y,bool onGround)
    {
        int a=onGround?Environment.TickCount/100%2:0;
        g.FillRectangle(Brushes.DarkSlateBlue,x+12,y+20,26,25);g.FillRectangle(Brushes.DarkSlateGray,x+10,y+8,30,19);
        g.FillRectangle(Brushes.Firebrick,x+8,y+10,34,7);g.FillRectangle(Brushes.Bisque,x+29,y+15,12,5);g.FillRectangle(Brushes.White,x+34,y+15,5,3);
        g.FillRectangle(Brushes.Firebrick,x+4,y+20,10,6);g.FillRectangle(Brushes.Firebrick,x+37,y+24,16,4);
        if(!onGround){g.FillRectangle(Brushes.DarkSlateBlue,x+4,y+37,12,13);g.FillRectangle(Brushes.DarkSlateBlue,x+25,y+42,11,12);g.FillRectangle(Brushes.Black,x+1,y+48,16,7);g.FillRectangle(Brushes.Black,x+24,y+52,17,7);}
        else {g.FillRectangle(Brushes.DarkSlateBlue,x+13,y+42,10, a==0?15:10);g.FillRectangle(Brushes.DarkSlateBlue,x+27,y+42,10,a==0?11:16);g.FillRectangle(Brushes.Black,x+7,y+(a==0?54:49),17,7);g.FillRectangle(Brushes.Black,x+29,y+(a==0?50:56),17,7);}
    }
    static void Monster(Graphics g,float x,float y)
    {
        g.FillRectangle(Brushes.DarkSlateBlue,x+7,y+12,42,40);g.FillRectangle(Brushes.MediumPurple,x+2,y+17,50,29);g.FillRectangle(Brushes.MediumPurple,x+10,y+5,34,18);
        g.FillRectangle(Brushes.Khaki,x+9,y,8,10);g.FillRectangle(Brushes.Khaki,x+38,y,8,10);
        g.FillRectangle(Brushes.White,x+13,y+19,10,10);g.FillRectangle(Brushes.White,x+34,y+19,10,10);g.FillRectangle(Brushes.Red,x+17,y+22,4,6);g.FillRectangle(Brushes.Red,x+37,y+22,4,6);
        g.FillRectangle(Brushes.Black,x+18,y+35,20,5);g.FillRectangle(Brushes.White,x+21,y+35,4,4);g.FillRectangle(Brushes.White,x+31,y+35,4,4);g.FillRectangle(Brushes.DarkSlateBlue,x+9,y+47,12,9);g.FillRectangle(Brushes.DarkSlateBlue,x+34,y+47,12,9);
    }
    static void Center(Graphics g,string s,Font f,Color c,Rectangle r){using var b=new SolidBrush(c);using var sf=new StringFormat{Alignment=StringAlignment.Center,LineAlignment=StringAlignment.Center};g.DrawString(s,f,b,r,sf);}
    static void Button(Graphics g,Rectangle r,string s){g.FillRectangle(Brushes.DarkSlateGray,r.X+4,r.Y+5,r.Width,r.Height);g.FillRectangle(Brushes.Goldenrod,r);g.DrawRectangle(new Pen(Color.DarkSlateGray,3),r);Center(g,s,new("Consolas",15,FontStyle.Bold),Color.DarkSlateGray,r);}
    protected override void OnMouseDown(MouseEventArgs e){base.OnMouseDown(e);if(mode==Mode.Title&&start.Contains(e.Location)||mode==Mode.Over&&restart.Contains(e.Location))StartGame();else if(pause.Contains(e.Location)){if(mode==Mode.Play)mode=Mode.Pause;else if(mode==Mode.Pause)mode=Mode.Play;}Invalidate();}
    protected override void OnKeyDown(KeyEventArgs e){base.OnKeyDown(e);if(e.KeyCode==Keys.Space)Jump();else if(e.KeyCode==Keys.Escape){if(mode==Mode.Play)mode=Mode.Pause;else if(mode==Mode.Pause)mode=Mode.Play;}else if(e.KeyCode==Keys.Enter){if(mode==Mode.Title||mode==Mode.Over)StartGame();else if(mode==Mode.Pause)mode=Mode.Play;}Invalidate();}
    protected override void OnFormClosed(FormClosedEventArgs e){timer.Stop();timer.Dispose();base.OnFormClosed(e);}
}
