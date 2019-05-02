using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using System;
using static Android.Views.View;
using AlertDialog = Android.App.AlertDialog;

namespace Paint
{
    class Handwriting
    {
        public static Bitmap blackBrush, brush;
        public static Color brushColor = Color.Black;
        public static float distance, width;
        public static void SetBrushColor(Color color)
        {
            if (color == brushColor)
            {
                return;
            }
            int brushHeight = blackBrush.Height, brushWidth = blackBrush.Width;
            for (int i = 0; i < brushWidth; i++)
            {
                for (int j = 0; j < brushHeight; j++)
                {
                    if (blackBrush.GetPixel(i, j) != Color.Transparent)
                    {
                        brush.SetPixel(i, j, color);
                    }

                }
            }
            brushColor = color;
        }
    }

    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        AlertDialog adColor;
        Bitmap bitmap;
        Button bColor;
        Canvas canvas;
        Color brushColor = Color.Black, eraserColor = Color.White;
        EditText etText;
        float prevX, prevY, size;
        ImageView ivCanvas;
        LinearLayout llPaint;
        Android.Graphics.Paint paint = new Android.Graphics.Paint()
        {
            StrokeWidth = 8,
            TextAlign = Android.Graphics.Paint.Align.Center,
            TextSize = 64
        };
        Android.Graphics.Paint.FontMetrics fontMetrics;
        RadioButton rbEraser, rbFill, rbHandwriting, rbPencil, rbText;
        SeekBar sbAlpha, sbBlue, sbGreen, sbRed, sbSize, sbWidth;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
            
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            builder.SetTitle("顏色");
            View view = LayoutInflater.From(this).Inflate(Resource.Layout.color, null);
            builder.SetView(view);
            adColor = builder.Create();
            
            bColor = FindViewById<Button>(Resource.Id.b_color);
            etText = FindViewById<EditText>(Resource.Id.et_text);
            fontMetrics = paint.GetFontMetrics();
            ivCanvas = FindViewById<ImageView>(Resource.Id.iv_canvas);
            llPaint = FindViewById<LinearLayout>(Resource.Id.ll_paint);
            rbEraser = FindViewById<RadioButton>(Resource.Id.rb_eraser);
            rbFill = FindViewById<RadioButton>(Resource.Id.rb_fill);
            rbHandwriting = FindViewById<RadioButton>(Resource.Id.rb_handwriting);
            rbPencil = FindViewById<RadioButton>(Resource.Id.rb_pencil);
            rbText = FindViewById<RadioButton>(Resource.Id.rb_text);
            sbAlpha = view.FindViewById<SeekBar>(Resource.Id.sb_alpha);
            sbBlue = view.FindViewById<SeekBar>(Resource.Id.sb_blue);
            sbGreen = view.FindViewById<SeekBar>(Resource.Id.sb_green);
            sbRed = view.FindViewById<SeekBar>(Resource.Id.sb_red);
            sbSize = FindViewById<SeekBar>(Resource.Id.sb_size);
            sbWidth = FindViewById<SeekBar>(Resource.Id.sb_width);

            adColor.DismissEvent += AdColor_DismissEvent;
            FindViewById<Button>(Resource.Id.b_clear).Click += BClear_Click;
            bColor.Click += BSolidColor_Click;
            FindViewById<Button>(Resource.Id.b_red).Click += BMaterialDesignColor_Click;
            FindViewById<Button>(Resource.Id.b_green).Click += BMaterialDesignColor_Click;
            FindViewById<Button>(Resource.Id.b_blue).Click += BMaterialDesignColor_Click;
            FindViewById<Button>(Resource.Id.b_yellow).Click += BMaterialDesignColor_Click;
            FindViewById<Button>(Resource.Id.b_cyan).Click += BMaterialDesignColor_Click;
            FindViewById<Button>(Resource.Id.b_magenta).Click += BMaterialDesignColor_Click;
            FindViewById<Button>(Resource.Id.b_black).Click += BMaterialDesignColor_Click;
            FindViewById<Button>(Resource.Id.b_white).Click += BMaterialDesignColor_Click;
            ivCanvas.Touch += IvCanvas_Touch;
            FindViewById<RadioGroup>(Resource.Id.rg_tool).CheckedChange += RgTool_CheckedChange;
            sbAlpha.ProgressChanged += SbColor_ProgressChanged;
            sbBlue.ProgressChanged += SbColor_ProgressChanged;
            sbGreen.ProgressChanged += SbColor_ProgressChanged;
            sbRed.ProgressChanged += SbColor_ProgressChanged;
            sbSize.ProgressChanged += SbSize_ProgressChanged;
            sbWidth.ProgressChanged += SbWidth_ProgressChanged;

            Handwriting.blackBrush = BitmapFactory.DecodeResource(Resources, Resource.Mipmap.brush);
            Handwriting.brush = Handwriting.blackBrush.Copy(Bitmap.Config.Argb8888, true);

        }

        private void AdColor_DismissEvent(object sender, EventArgs e)
        {
            if (rbHandwriting.Checked)
            {
                Handwriting.SetBrushColor(paint.Color);
            }
        }

        private void BClear_Click(object sender, EventArgs e)
        {
            if (bitmap != null)
            {
                canvas.DrawColor(eraserColor);
                ivCanvas.SetImageBitmap(bitmap);
            }
        }

        private void BMaterialDesignColor_Click(object sender, EventArgs e)
        {
            Color color = new Color(((Button)sender).TextColors.DefaultColor);
            SetColor(color.R, color.G, color.B);
            if (rbHandwriting.Checked)
            {
                Handwriting.SetBrushColor(paint.Color);
            }
        }

        private void BSolidColor_Click(object sender, EventArgs e)
        {
            adColor.Show();
        }

        private void IvCanvas_Touch(object sender, TouchEventArgs e)
        {
            float x = e.Event.GetX(), y = e.Event.GetY();
            if (bitmap == null)
            {
                bitmap = Bitmap.CreateBitmap(ivCanvas.Width, ivCanvas.Height, Bitmap.Config.Argb8888);
                canvas = new Canvas(bitmap);
                size = (float)Math.Sqrt(Math.Pow(ivCanvas.Width, 2) + Math.Pow(ivCanvas.Height, 2));
            }
            if (e.Event.Action == MotionEventActions.Down)
            {
                prevX = x;
                prevY = y;
            }

            if (rbPencil.Checked)
            {
                switch (e.Event.Action)
                {
                    case MotionEventActions.Down:
                        canvas.DrawCircle(x, y, sbWidth.Progress, paint);
                        break;
                    case MotionEventActions.Move:
                        canvas.DrawLine(prevX, prevY, x, y, paint);
                        canvas.DrawCircle(x, y, sbWidth.Progress, paint);
                        break;
                }
            }
            else if (rbHandwriting.Checked)
            {
                switch (e.Event.Action)
                {
                    case MotionEventActions.Down:
                        Handwriting.distance = 0;
                        Handwriting.width = 0;
                        break;
                    case MotionEventActions.Move:
                        float d = (float)Math.Sqrt(Math.Pow(x - prevX, 2) + Math.Pow(y - prevY, 2)),
                            a = d / (float)Math.Pow(d, 2),
                            w = 0,
                            width = (float)Math.Pow(1 - d / size, 10) * paint.StrokeWidth,
                            xpd = (x - prevX) / d, ypd = (y - prevY) / d;
                        if (width >= Handwriting.width)
                        {
                            for (float f = 0; f < d; f += 4)
                            {
                                w = a * (float)Math.Pow(f, 2) / d * (width - Handwriting.width) + Handwriting.width;
                                canvas.DrawBitmap(Handwriting.brush, new Rect(0, 0, 192, 192), new Rect((int)(xpd * f + prevX - w), (int)(ypd * f + prevY - w), (int)(xpd * f + prevX + w), (int)(ypd * f + prevY + w)), paint);
                            }
                        }
                        else
                        {
                            for (float f = 0; f < d; f += 4)
                            {
                                w = (float)Math.Sqrt(f / a) / d * (width - Handwriting.width) + Handwriting.width;
                                canvas.DrawBitmap(Handwriting.brush, new Rect(0, 0, 192, 192), new Rect((int)(xpd * f + prevX - w), (int)(ypd * f + prevY - w), (int)(xpd * f + prevX + w), (int)(ypd * f + prevY + w)), paint);
                            }
                        }
                        Handwriting.distance = d;
                        Handwriting.width = w;
                        break;
                    case MotionEventActions.Up:
                        break;
                }
            }
            else if (rbFill.Checked)
            {
                if (e.Event.Action == MotionEventActions.Down)
                {
                    canvas.DrawPaint(paint);
                }
            }
            else if (rbText.Checked)
            {
                fontMetrics = paint.GetFontMetrics();
                canvas.DrawText(etText.Text, x, y - fontMetrics.Top / 2 - fontMetrics.Bottom / 2, paint);
            }
            else if (rbEraser.Checked)
            {
                switch (e.Event.Action)
                {
                    case MotionEventActions.Down:
                        canvas.DrawCircle(x, y, sbWidth.Progress, paint);
                        break;
                    case MotionEventActions.Move:
                        canvas.DrawLine(prevX, prevY, x, y, paint);
                        canvas.DrawCircle(x, y, sbWidth.Progress, paint);
                        break;
                }
            }

            if (e.Event.Action == MotionEventActions.Move)
            {
                prevX = x;
                prevY = y;
            }
            ivCanvas.SetImageBitmap(bitmap);
        }

        private void RgTool_CheckedChange(object sender, RadioGroup.CheckedChangeEventArgs e)
        {
            if (!FindViewById<RadioButton>(e.CheckedId).Checked)
            {
                return;
            }
            if (e.CheckedId == Resource.Id.rb_eraser)
            {
                SetColor(eraserColor);
            }
            else
            {
                SetColor(brushColor);
            }
            if (e.CheckedId == Resource.Id.rb_handwriting)
            {
                Handwriting.SetBrushColor(paint.Color);
            }
            switch (e.CheckedId)
            {
                default:
                    SelectTab(Resource.Id.sb_width);
                    break;
                case Resource.Id.rb_fill:
                    SelectTab();
                    break;
                case Resource.Id.rb_text:
                    SelectTab(new int[] {
                        Resource.Id.et_text,
                        Resource.Id.sb_size
                    });
                    break;
            }
        }

        private void SbColor_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            Color color = new Color(sbRed.Progress, sbGreen.Progress, sbBlue.Progress, sbAlpha.Progress);
            paint.Color = color;
            bColor.SetTextColor(color);
            if (!rbEraser.Checked)
            {
                brushColor = color;
            }
            else
            {
                eraserColor = color;
            }
        }

        private void SbSize_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            paint.TextSize = e.Progress;
        }

        private void SbWidth_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            paint.StrokeWidth = e.Progress * 2;
        }

        private void SelectTab()
        {
            SelectTab(new int[0]);
        }

        private void SelectTab(int id)
        {
            SelectTab(new int[] {
                id
            });
        }

        private void SelectTab(int[] ids)
        {
            sbWidth.Visibility = ViewStates.Gone;
            etText.Visibility = ViewStates.Gone;
            sbSize.Visibility = ViewStates.Gone;
            foreach (int i in ids)
            {
                FindViewById<View>(i).Visibility = ViewStates.Visible;
            }
        }

        private void SetColor(Color color)
        {
            SetColor(color.A, color.R, color.G, color.B);
        }

        private void SetColor(byte r, byte g, byte b)
        {
            sbRed.Progress = r;
            sbGreen.Progress = g;
            sbBlue.Progress = b;
        }

        private void SetColor(byte a, byte r, byte g, byte b)
        {
            sbAlpha.Progress = a;
            SetColor(r, g, b);
        }
    }
}