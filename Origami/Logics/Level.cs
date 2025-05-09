﻿using Android.Graphics;
using Android.Widget;
using Java.Lang;
using System.Xml;
using Xamarin.Essentials;

namespace Origami.Logics
{
    public class Level
    {
        public static bool FoldsDenied = false;

        public static Color FoldLineColor;

        public struct LevelRating
        {
            public bool unlocked;
            public int stars;
        }

        public LevelRating rating;

        PaperSheet FoldedSheet {
            get { return foldedSheet; }
            set 
            {
                foldedSheet = value;
                GameActivity.Instance?.RedrawField();
            } 
        }
        PaperSheet foldedSheet;

        PaperSheet[] fold_stages;

        int max_folds;
        int last_fold_id;

        int last_help_id;

        string name;
        int id;

        const float SHEET_PADDING = 0.1f;
        const float MIN_MOVE = 0.0005f;

        FloatLine[] folds;

        enum LevelType
        {
            RESULT,
            SEQUENCE
        }
        const LevelType type = LevelType.RESULT;

        LineSegment[] resultOutline;
        int currOutline = 0;

        #region load resources

        bool xml_commas_replace;

        public Level(XmlElement level_element, int level_number)
        {
            name = level_element.GetAttribute("name");
            id = level_number;

            string comma_check = "0,1";
            float.TryParse(comma_check, out float comma_check_float);
            xml_commas_replace = !(comma_check_float == 0.1f);

            foreach (XmlElement data in level_element)
                if(data.Name == "folds")
                    LoadFolds(data);
                else if (data.Name == "result")
                    LoadResultOutline(data);

            fold_stages = new PaperSheet[max_folds + 1];
            fold_stages[0] = new PaperSheet();
            fold_stages[0].LoadQuad(SHEET_PADDING);
            last_fold_id = 0;

            #if DEBUG
                if (LogicCore.level_folds_count.ContainsKey(max_folds))
                    LogicCore.level_folds_count[max_folds]++;
                else
                    LogicCore.level_folds_count.Add(max_folds, 1);
            #endif

            FoldedSheet = fold_stages[0];
        }

        void LoadFolds(XmlElement folds_xml)
        {
            max_folds = folds_xml.ChildNodes.Count;
            folds = new FloatLine[max_folds];
            int i = 0;
            foreach (XmlElement fold in folds_xml)
            {
                string x0_str = fold.GetAttribute("x0");
                string x1_str = fold.GetAttribute("x1");
                string y0_str = fold.GetAttribute("y0");
                string y1_str = fold.GetAttribute("y1");

                if(xml_commas_replace)
                {
                    x0_str = x0_str.Replace(',', '.');
                    x1_str = x1_str.Replace(',', '.');
                    y0_str = y0_str.Replace(',', '.');
                    y1_str = y1_str.Replace(',', '.');
                }

                float.TryParse(x0_str, out float x0);
                float.TryParse(y0_str, out float y0);
                float.TryParse(x1_str, out float x1);
                float.TryParse(y1_str, out float y1);

                Vector2 start = new Vector2(x0, y0);
                Vector2 end = new Vector2(x1, y1);
                folds[i++] = new FloatLine(start, end - start);
            }
        }

        void LoadResultOutline(XmlElement result_xml)
        {
            resultOutline = new LineSegment[result_xml.ChildNodes.Count];

            Vector2[] points = new Vector2[result_xml.ChildNodes.Count];
            int i = 0; // Point counter.
            foreach (XmlElement point in result_xml)
            {
                string x_str = point.GetAttribute("x");
                string y_str = point.GetAttribute("y");

                if (xml_commas_replace)
                {
                    x_str = x_str.Replace(',', '.');
                    y_str = y_str.Replace(',', '.');
                }

                float.TryParse(x_str, out float x);
                float.TryParse(y_str, out float y);

                points[i] = new Vector2(x, y);
                // Apply sheet padding
                points[i] = points[i] * (1.0f - 2.0f * SHEET_PADDING);
                points[i] = points[i] + new Vector2(SHEET_PADDING, SHEET_PADDING);

                i++;
            }

            for (i = 0; i < points.Length; i++)
                resultOutline[i] = new LineSegment(points[i], points[(i + 1) % points.Length]);
        }

        #endregion

        public void Undo()
        {
            if (last_fold_id == 0)
                return;

            FoldedSheet = fold_stages[--last_fold_id];

            UpdateStates();
        }

        public void ResetFoldState()
        {
            last_help_id = 0;
            last_fold_id = 0;
            currOutline = 0;
            foldedSheet = fold_stages[0];
        }

        public void ResetAndRefresh()
        {
            ResetFoldState();

            if (GameActivity.Instance == null)
                return;

            FoldedSheet = fold_stages[0];

            RecreateCorrectPercentThread();

            UpdateStates();
        }

        public void UpdateStates()
        {
            GameActivity.Instance.SetFoldLimit(fold_stages.Length - 1);
            GameActivity.Instance.SetFolds(last_fold_id);
            GameActivity.Instance.SetLevelName(name);
            GameActivity.Instance.SetLevelNumber(id);
        }

        public void Help()
        {
            last_help_id++;

            for(int i = 0; i < last_help_id; i++)
                fold_stages[i + 1] = fold_stages[i].Fold(folds[i]);

            last_fold_id = last_help_id;
            FoldedSheet = fold_stages[last_help_id];
                
            if (last_fold_id == fold_stages.Length - 1)
                MainMenuActivity.Instance.core.CurrentLevelCompleted();

            UpdateStates();
        }

        #region input

        Vector2 fold_start_pos;

        public void TouchStart(Vector2 position_normalized)
        {
            if (FoldsDenied || last_fold_id == fold_stages.Length - 1)
                return;

            fold_start_pos = position_normalized;
        }

        public void TouchMove(Vector2 position_normalized)
        {
            if (FoldsDenied || last_fold_id == fold_stages.Length - 1)
                return;

            if((position_normalized - fold_start_pos).LengthSqr() <= MIN_MOVE) // Click but not move
                return;

            FloatLine fold_line = new FloatLine();
            fold_line.passThrough = (fold_start_pos + position_normalized) * 0.5f;
            Vector2 fold_vec = fold_start_pos - position_normalized;
            fold_line.angle = new Vector2(-fold_vec.y, fold_vec.x);

            var new_folded = fold_stages[last_fold_id].Fold(fold_line);
            if (new_folded != null)
                FoldedSheet = new_folded;
        }

        public void TouchEnd(Vector2 position_normalized)
        {
            if (FoldsDenied || last_fold_id == fold_stages.Length - 1)
                return;

            if ((position_normalized - fold_start_pos).LengthSqr() <= MIN_MOVE) // Click but not move
                return;
                
            fold_stages[++last_fold_id] = FoldedSheet;

            if(last_fold_id == fold_stages.Length - 1)
                MainMenuActivity.Instance.core.CurrentLevelCompleted();

            UpdateStates();
        }

        #endregion

        #region render

        // For GetCorrectPercent purpose.
        static Bitmap last_bitmap;
        static Bitmap field_bitmap;

        public void RenderField(ImageView image_view)
        {
            if(field_bitmap == null)
                field_bitmap = Bitmap.CreateBitmap(image_view.Width, image_view.Height, Bitmap.Config.Argb8888);

            field_bitmap.EraseColor(0);

            var canvas = new Canvas(field_bitmap);

            FoldedSheet.Render(canvas);

            if (last_bitmap == null)
                last_bitmap = Bitmap.CreateBitmap(field_bitmap);
            else
            {
                last_bitmap.EraseColor(0);
                FoldedSheet.Render(new Canvas(last_bitmap));
            }

            RenderResultOutline(canvas);

            image_view.SetImageBitmap(field_bitmap);
        }

        void RenderResultOutline(Canvas canvas)
        {
            float width = canvas.Width;
            float height = canvas.Height;

            Paint paint = new Paint();
            paint.Color = FoldLineColor;
            paint.SetStyle(Paint.Style.Stroke);
            paint.StrokeCap = Paint.Cap.Round;
            paint.StrokeWidth = 5;
            paint.SetPathEffect(new DashPathEffect(new float[] { 10, 10 }, 0));

            foreach (var line in resultOutline)
                canvas.DrawLine((line.start.x * width), (line.start.y * height), (line.end.x * width), (line.end.y * height), paint);
        }

        #endregion

        #region correct percent

        static Thread correctPercentThread;

        public static float lastCorrectPercent = 1;

        static void RecreateCorrectPercentThread()
        {
            if (correctPercentThread != null)
                correctPercentThread.Interrupt();

            correctPercentThread = new Thread(CorrectPercentComputer);
            correctPercentThread.Priority = Thread.MinPriority;
            correctPercentThread.Start();
        }

        static void CorrectPercentComputer()
        {
            while (true)
            {
                lastCorrectPercent = GetCorrectPercent();

                var score = (int)Math.Ceil(lastCorrectPercent * 100);
                if (GameActivity.Instance.last_score != score)
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        GameActivity.Instance.SetScore(score);
                    });
            }
        }

        public static float GetCorrectPercent()
        {
            if (last_bitmap == null)
                return 0;

            var curr_level = MainMenuActivity.Instance.core.CurrentLevel();
            return curr_level.fold_stages[curr_level.last_fold_id].GetCorrectPercent(last_bitmap, curr_level.resultOutline);
        }

        #endregion
    }
}