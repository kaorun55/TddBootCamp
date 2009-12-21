using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TddBootCamp;
using System.Threading;

namespace TddBootCampTest
{
	/// <summary>
	/// UnitTest1 の概要の説明
	/// </summary>
	[TestClass]
	public class LruCacheTest
	{
		public LruCacheTest()
		{
			//
			// TODO: コンストラクター ロジックをここに追加します
			//
		}

		private TestContext testContextInstance;

		/// <summary>
		///現在のテストの実行についての情報および機能を
		///提供するテスト コンテキストを取得または設定します。
		///</summary>
		public TestContext TestContext
		{
			get
			{
				return testContextInstance;
			}
			set
			{
				testContextInstance = value;
			}
		}

		#region 追加のテスト属性
		//
		// テストを作成する際には、次の追加属性を使用できます:
		//
		// クラス内で最初のテストを実行する前に、ClassInitialize を使用してコードを実行してください
		// [ClassInitialize()]
		// public static void MyClassInitialize(TestContext testContext) { }
		//
		// クラス内のテストをすべて実行したら、ClassCleanup を使用してコードを実行してください
		// [ClassCleanup()]
		// public static void MyClassCleanup() { }
		//
		// 各テストを実行する前に、TestInitialize を使用してコードを実行してください
		// [TestInitialize()]
		// public void MyTestInitialize() { }
		//
		// 各テストを実行した後に、TestCleanup を使用してコードを実行してください
		// [TestCleanup()]
		// public void MyTestCleanup() { }
		//
		#endregion

		[TestMethod]
		public void インスタンスを作る()
		{
			LruCache lruCache = new LruCache();
			Assert.IsNotNull(lruCache);
		}

		[TestMethod]
		public void abcを入れてabcが出てくる()
		{
			LruCache lruCache = new LruCache();
			lruCache.Put("a", "abc");
			Assert.AreEqual("abc", lruCache.Get("a"));
		}

		[TestMethod]
		public void zxyを入れてzxyが出てくる()
		{
			LruCache lruCache = new LruCache();
			lruCache.Put("b", "zxy");
			Assert.AreEqual("zxy", lruCache.Get("b"));
		}

		[TestMethod]
		public void キーが重複したものを入れると書き変わる()
		{
			LruCache lruCache = new LruCache();
			lruCache.Put("a", "abc");
			lruCache.Put("a", "123");
			Assert.AreEqual("123", lruCache.Get("a"));
		}


		[TestMethod]
		public void 三個目を入れると最初に入れておいたものが消える()
		{
			LruCache lruCache = new LruCache();
			lruCache.Put("a", "abc");
			lruCache.Put("b", "zxy");
			lruCache.Put("3", "3つめ");
			Assert.IsNull(lruCache.Get("a"));
		}

		[TestMethod]
		public void 最初に入れておいたものを使ったら_三個目を入れた時に消えるのは二番目に入れたもの()
		{
			LruCache lruCache = new LruCache();
			lruCache.Put("a", "abc");
			lruCache.Put("b", "zxy");
			lruCache.Get("a");

			lruCache.Put("3", "3つめ");
			Assert.IsNull(lruCache.Get("b"));
		}

		[TestMethod]
		public void 空の状態で取り出すとnullが戻ってくる()
		{
			LruCache lruCache = new LruCache();
			Assert.IsNull(lruCache.Get("a"));
		}

		[TestMethod]
		public void サイズ3の時に_四個目を入れると最初に入れておいたものが消える()
		{
			LruCache lruCache = new LruCache(3);
			lruCache.Put("a", "abc");
			lruCache.Put("b", "zxy");
			lruCache.Put("3", "3つめ");
			Assert.IsNotNull(lruCache.Get("a"));
			lruCache.Put("4", "4つめ");
			Assert.IsNull(lruCache.Get("b"));
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void サイズ0を指定してArgumentOutOfRangeExceptionが発生する()
		{
			LruCache lruCache = new LruCache(0);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void キーでnullを指定するとArgumentNullExceptionが発生する()
		{
			LruCache lruCache = new LruCache(2);
			lruCache.Put(null, "aaa");
		}

		[TestMethod]
		public void キャッシュサイズを増やした時に内容は変わらずサイズだけ増える()
		{
			LruCache lruCache = new LruCache(2);
			lruCache.Put("a", "aaa");
			lruCache.Put("b", "abc");
			lruCache.Resize(5);
			Assert.AreEqual(5, lruCache.MaxSize);
			Assert.AreEqual("aaa", lruCache.Get("a"));
			Assert.AreEqual("abc", lruCache.Get("b"));
		}

		[TestMethod]
		public void キャッシュサイズを減らした時に内容が消去されサイズも減る()
		{
			LruCache lruCache = new LruCache(5);
			lruCache.Put("a", "aaa");
			lruCache.Put("b", "abc");
			lruCache.Put("c", "333");
			lruCache.Put("d", "444");
			lruCache.Put("e", "555");
			lruCache.Resize(1);
			Assert.AreEqual(1, lruCache.MaxSize);
			Assert.AreEqual("555", lruCache.Get("e"));
			Assert.IsNull(lruCache.Get("a"));
			Assert.IsNull(lruCache.Get("b"));
			Assert.IsNull(lruCache.Get("c"));
			Assert.IsNull(lruCache.Get("d"));
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void キャッシュサイズを0以下に設定すると例外が発生する()
		{
			LruCache lruCache = new LruCache(5);
			lruCache.Resize(0);
		}

		[TestMethod]
		public void 削除スパンを設定してインスタンスを生成する()
		{
			LruCache lruCache = new LruCache(5, 1000);
			Assert.IsNotNull(lruCache);
		}

		[TestMethod]
		public void 削除スパンを設定してそれを超えると要素が消える()
		{
			LruCache lruCache = new LruCache(5, 100);
			lruCache.Put("a", "aaa");
			Thread.Sleep(1000);
			Assert.IsNull(lruCache.Get("a"));
		}

		[TestMethod]
		public void 削除スパンを設定してそれを超えて設定すると前のは消える()
		{
			LruCache lruCache = new LruCache(5, 100);
			lruCache.Put("a", "aaa");
			Thread.Sleep(1000);
			lruCache.Put("b", "bbb");

			Assert.AreEqual(1, lruCache._orderedList.Count);
			Assert.IsNull(lruCache.Get("a"));
			Assert.AreEqual("bbb", lruCache.Get("b"));
		}

		[TestMethod]
		public void GetとPutがスレッドセーフである()
		{
			LruCache lruCache = new LruCache(5, 1);
			Action action1 = () => {
				for (int i = 0; i < 1000 * 1000; i++)
				{
					lruCache.Put("a", "abc");
					Assert.AreEqual("abc", lruCache.Get("a"));
				}
			};
			Action action2 = () => {
				for (int i = 0; i < 1000 * 1000; i++)
				{
					lruCache.Put("a", "abc");
					Assert.AreEqual("abc", lruCache.Get("a"));
				}
			};
			action1.BeginInvoke(null, null);
			action2.BeginInvoke(null, null);
		}
	}
}
