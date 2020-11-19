using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using BeatSaviorData.Trackers;
using HMUI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaviorData
{
	class ScoreGraphUI : BSMLResourceViewController
	{
		public override string ResourceName => "BeatSaviorData.UI.Views.ScoreGraphView.bsml";

		public SongData data;
		private int lastSongBeat = 0;
		private float width = 90, height = 45, offsetx = 10f, offsety = 5f, scoreOffset, modifiersMultiplier;
		private bool won = true;

		private List<float> misses = new List<float>();
		Dictionary<float, float> graph;

		[UIObject("graph")]
		private GameObject graphObject;

		[UIComponent("noTrackerText")]
		private readonly CurvedTextMeshPro noTrackerText;
		[UIComponent("title")]
		private readonly CurvedTextMeshPro titleText;

		[UIAction("#post-parse")]
		public void SetDataToUI()
		{
			if(!SettingsMenu.instance.EnableDeepTrackers)
			{
				noTrackerText.gameObject.SetActive(true);
				return;
			} else
			{
				noTrackerText.gameObject.SetActive(false);
			}

			List<Note> notes = (data.deepTrackers["noteTracker"] as NoteTracker).notes;
			graph = (data.trackers["scoreGraphTracker"] as ScoreGraphTracker).graph;
			float lastGraphEntry = -1;

			GetOffsets(notes);
			lastSongBeat = Mathf.CeilToInt(data.songDuration);
			won = (data.trackers["winTracker"] as WinTracker).won;
			titleText.text = data.songName;
			titleText.enableAutoSizing = true;
			titleText.fontSizeMin = 1;
			titleText.fontSizeMax = 10;
			modifiersMultiplier = (data.trackers["scoreTracker"] as ScoreTracker).modifiersMultiplier;

			CreateHorizontalLabels();

			foreach(float f in graph.Keys)
			{
				if (lastGraphEntry != -1)
				{
					CreateGraphLine((lastGraphEntry, graph[lastGraphEntry]), (f, graph[f]), Color.white);
				}
				lastGraphEntry = f;
			}

			CreateVerticalLabels();
		} 

		private void CreateVerticalLabels()
		{
			List<float> drops = new List<float>();
			int labelsLeft = 3;

			if(misses.Count != labelsLeft)
				drops = FindScoreDrops();

			if (misses.Count < 4)
			{
				for (int i = 0; i < misses.Count; i++)
				{
					CreateVerticalLabelLine(misses[i], graph[misses[i]], new Color(1, 1, 1, .75f), "Missed");
					labelsLeft--;
				}
			}

			for (int i = 0; i < drops.Count && labelsLeft > 0; i++)
			{
				if (misses.Count < 4 && !misses.Contains(drops[i]) || misses.Count >= 4)
				{
					CreateVerticalLabelLine(drops[i], graph[drops[i]], new Color(1, 1, 1, .75f), "Score drop");
					labelsLeft--;
				}
			}

			if (!won)
				CreateVerticalLabelLine(graph.Last().Key, graph.Last().Value, Color.red, "Failed");
		}

		private List<float> FindScoreDrops()
		{
			int dropWindow = 2;     // in seconds
			int closestToDropWindow = (int)graph.First().Key, dropSpacing = 4;
			float diff, dropDelta = -0.01f;
			List<(float, float)> drops = new List<(float, float)>();
			List<float> res = new List<float>();

			if (scoreOffset == 0.93f)
				dropDelta = 0.0025f;	// 0.25%
			else if (scoreOffset == 0.8f)
				dropDelta = 0.005f;		// 0.5%

			for (int i = (int) graph.First().Key + dropWindow + 5; i < graph.Last().Key; i++)
			{
				if (graph.ContainsKey(i - dropWindow))
					closestToDropWindow = i - dropWindow;

				if (graph.ContainsKey(i))
				{
					diff = graph[i] - graph[closestToDropWindow];
					if (diff < dropDelta)
					{
						drops.Add((i, diff));
						i += dropSpacing;
					}
				}
			}

			drops = drops.OrderBy(e => e.Item2).Take(3).ToList();

			while (res.Count < 3 && drops.Count > 0)
			{
				res.Add(drops.First().Item1);
				drops.RemoveAt(0);
			}

			return res;
		}

		private void CreateHorizontalLabels()
		{
			float increment = 0.1f;

			if (scoreOffset == 0.93f)
				increment = 0.01f;
			else if (scoreOffset == 0.8f)
				increment = 0.05f;

			for (float i = 0 ; i <= 1 + increment - 0.001f; i += increment)
			{
				if (i >= scoreOffset - 0.001f)
				{
					CreateHorizontalLabelLine(i, new Color(1, 0.2f + i * 0.8f, 0.2f + i * 0.8f, .75f));
				}
			}
		}

		private void CreateVerticalLabelLine(float x, float y, Color color, string text)
		{
			void CreateLabelText(string _text)
			{
				GameObject go = new GameObject("LabelText", typeof(CurvedTextMeshPro));
				RectTransform rt = go.GetComponent<RectTransform>();
				CurvedTextMeshPro tmp = go.GetComponent<CurvedTextMeshPro>();

				go.transform.SetParent(graphObject.transform, false);
				tmp.text = _text;
				tmp.color = color;
				tmp.alignment = TextAlignmentOptions.Center;
				tmp.fontSize = 3;

				rt.anchorMin = Vector2.zero;
				rt.anchorMax = Vector2.zero;
				rt.sizeDelta = new Vector2(12, 10);
				rt.anchoredPosition = new Vector2((x / lastSongBeat) * width + offsetx, -2f);
			}

			Vector2 pos1v = new Vector2((x / lastSongBeat) * width + offsetx, 3f),
					pos2v = new Vector2((x / lastSongBeat) * width + offsetx, ((y - scoreOffset) / (1 - scoreOffset)) * height + offsety - 2f);

			CreateLine("LabelLine", pos1v, pos2v, color, 0.1f);
			CreateLabelText(text + " at " + $"{Math.Floor(x / 60):N0}:{Math.Floor(x % 60):00}");
		}

		private void CreateHorizontalLabelLine(float value, Color color)
		{
			void CreateLabelText(string text)
			{
				GameObject go = new GameObject("LabelText", typeof(CurvedTextMeshPro));
				RectTransform rt = go.GetComponent<RectTransform>();
				CurvedTextMeshPro tmp = go.GetComponent<CurvedTextMeshPro>();

				go.transform.SetParent(graphObject.transform, false);
				tmp.text = text;
				tmp.color = color;
				tmp.fontSize = 3;

				rt.anchorMin = Vector2.zero;
				rt.anchorMax = Vector2.zero;
				rt.pivot = new Vector2(0, 1);
				rt.sizeDelta = new Vector2(10, 5);
				rt.anchoredPosition = new Vector2(3f, ((value - scoreOffset) / (1 - scoreOffset)) * height + offsety);
			}

			Vector2 pos1v = new Vector2(4f, ((value - scoreOffset) / (1 - scoreOffset)) * height + offsety),
					pos2v = new Vector2(width + 13f, ((value - scoreOffset) / (1 - scoreOffset)) * height + offsety);

			CreateLine("LabelLine", pos1v, pos2v, color, 0.25f);
			CreateLabelText((value * 100 * modifiersMultiplier).ToString("0") + " %");
		}

		private void CreateGraphLine((float, float) pos1, (float, float) pos2, Color color)
		{
			Vector2 pos1v = new Vector2((pos1.Item1 / lastSongBeat) * width + offsetx, ((pos1.Item2 - scoreOffset) / (1 - scoreOffset)) * height + offsety),
					pos2v = new Vector2((pos2.Item1 / lastSongBeat) * width + offsetx, ((pos2.Item2 - scoreOffset) / (1 - scoreOffset)) * height + offsety);

			CreateLine("GraphLine", pos1v, pos2v, color, 0.75f);
		}

		private void CreateLine(string name, Vector2 pos1v, Vector2 pos2v, Color color, float lineWidth)
		{
			GameObject go = new GameObject(name);

			Vector2 dir = (pos2v - pos1v).normalized;
			float distance = Vector2.Distance(pos1v, pos2v);

			ImageView image = go.AddComponent<ImageView>();
			image.sprite = BeatSaberMarkupLanguage.Utilities.ImageResources.WhitePixel;
			image.material = BeatSaberMarkupLanguage.Utilities.ImageResources.NoGlowMat;
			image.color = color;

			go.transform.SetParent(graphObject.transform, false);
			RectTransform rt = go.GetComponent<RectTransform>();

			rt.anchorMin = Vector2.zero;
			rt.anchorMax = Vector2.zero;
			rt.sizeDelta = new Vector2(distance, lineWidth);
			rt.anchoredPosition = pos1v + dir * distance * .5f;
			rt.localEulerAngles = new Vector3(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
		}

		private void GetOffsets(List<Note> notes)
		{
			foreach (Note n in notes)
			{
				if (n.IsAMiss() && misses.Count < 4)
					misses.Add((float)Math.Truncate(n.time));
			}

			scoreOffset = graph.Min(e => e.Value);

			if (scoreOffset >= 0.93f)
				scoreOffset = 0.93f;
			else if (scoreOffset >= 0.85f)
				scoreOffset = 0.8f;
			else
				scoreOffset -= 0.1f;
		}

	}
}
