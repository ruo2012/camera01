namespace Tiger.Video.VFW
{
	using System;
	using System.Drawing;
	using System.Drawing.Imaging;
	using System.Runtime.InteropServices;

	/// <summary>
	/// Writing AVI files using Video for Windows
	/// 
	/// Note: I am stucked with non even frame width. AVIs with non even
	/// width are playing well in WMP, but not in BSPlayer (it's for "DIB " codec).
	/// Some other codecs does not work with non even width or height at all.
	/// 
	/// </summary>
	public class AVIWriter : IDisposable
	{
		private IntPtr	file;
		private IntPtr	stream;
		private IntPtr	streamCompressed;
		private IntPtr	buf = IntPtr.Zero;

		private int		width;
		private int		height;
		private int		stride;
		private string	codec = "DIB ";
		private int		quality = -1;
		private int		rate = 25;
		private int		position;

		//当前位置属性
		public int CurrentPosition
		{
			get { return position; }
		}
		// 宽度
		public int Width
		{
			get
			{
				return (buf != IntPtr.Zero) ? width : 0;
			}
		}
		// 高度
		public int Height
		{
			get
			{
				return (buf != IntPtr.Zero) ? height : 0;
			}
		}
		//代码属性
		public string Codec
		{
			get { return codec; }
			set { codec = value; }
		}
		//质量属性
		public int Quality
		{
			get { return quality; }
			set { quality = value; }
		}
		// 帧率属性
		public int FrameRate
		{
			get { return rate; }
			set { rate = value; }
		}


		// 构造器
		public AVIWriter()
		{
			Win32.AVIFileInit();
		}
		public AVIWriter(string codec) : this()
		{
			this.codec = codec;
		}

		// 销毁器
		~AVIWriter()
		{
			Dispose(false);
		}

		//释放所有资源
		public void Dispose()
		{
			Dispose(true);
			// 从初始化的队列中移除
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				//释放所有资源
			}

			Close();
			Win32.AVIFileExit();
		}

		//创建一个AVI文件
		public void Open(string fname, int width, int height)
		{
			// 关闭当前文件
			Close();

			// 计算跨度
			stride = width * 3;
			int r = stride % 4;
			if (r != 0)
				stride += (4 - r);

			// 创建一个新文件
			if (Win32.AVIFileOpen(out file, fname, Win32.OpenFileMode.Create | Win32.OpenFileMode.Write, IntPtr.Zero) != 0)
				throw new ApplicationException("打开文件失败！");

			this.width = width;
			this.height = height;

			// 文件流属性描述
			Win32.AVISTREAMINFO info = new Win32.AVISTREAMINFO();

			info.fccType	= Win32.mmioFOURCC("vids");
			info.fccHandler	= Win32.mmioFOURCC(codec);
			info.dwScale	= 1;
			info.dwRate		= rate;
			info.dwSuggestedBufferSize = stride * height;

			// 创建流
			if (Win32.AVIFileCreateStream(file, out stream, ref info) != 0)
				throw new ApplicationException("创建流失败！");

			// 压缩选项描述
			Win32.AVICOMPRESSOPTIONS opts = new Win32.AVICOMPRESSOPTIONS();

			opts.fccHandler	= Win32.mmioFOURCC(codec);
			opts.dwQuality	= quality;

			//
			// Win32.AVISaveOptions(stream, ref opts, IntPtr.Zero);
			
			// 创建压缩流
			if (Win32.AVIMakeCompressedStream(out streamCompressed, stream, ref opts, IntPtr.Zero) != 0)
				throw new ApplicationException("创建压缩流失败！");

			// 描述帧格式
			Win32.BITMAPINFOHEADER bih = new Win32.BITMAPINFOHEADER();

			bih.biSize		= Marshal.SizeOf(bih.GetType());
			bih.biWidth		= width;
			bih.biHeight	= height;
			bih.biPlanes	= 1;
			bih.biBitCount	= 24;
			bih.biSizeImage	= 0;
			bih.biCompression = 0; // BI_RGB

			// 设置帧格式
			if (Win32.AVIStreamSetFormat(streamCompressed, 0, ref bih, Marshal.SizeOf(bih.GetType())) != 0)
				throw new ApplicationException("Failed creating compressed stream");

			// 请求空闲内存
			buf = Marshal.AllocHGlobal(stride * height);

			position = 0;
		}

		//关闭文件
		public void Close()
		{
			// 释放内存
			if (buf != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(buf);
				buf = IntPtr.Zero;
			}
			//释放压缩流
			if (streamCompressed != IntPtr.Zero)
			{
				Win32.AVIStreamRelease(streamCompressed);
				streamCompressed = IntPtr.Zero;
			}
			//释放流
			if (stream != IntPtr.Zero)
			{
				Win32.AVIStreamRelease(stream);
				stream = IntPtr.Zero;
			}
			//释放文件
			if (file != IntPtr.Zero)
			{
				Win32.AVIFileRelease(file);
				file = IntPtr.Zero;
			}
		}

		// 把新的一帧加入到AVI
		public void AddFrame(System.Drawing.Bitmap bmp)
		{
			// 检查图像尺寸
			if ((bmp.Width != width) || (bmp.Height != height))
				throw new ApplicationException("Invalid image dimension");

			// 锁定图像
			BitmapData	bmData = bmp.LockBits(
				new Rectangle(0, 0, width, height),
				ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

			// 拷贝图像数据
			int srcStride = bmData.Stride;
			int dstStride = stride;

			int src = bmData.Scan0.ToInt32() + srcStride * (height - 1);
			int dst = buf.ToInt32();

			for (int y = 0; y < height; y++)
			{
				Win32.memcpy(dst, src, dstStride);
				dst += dstStride;
				src -= srcStride;
			}

			// 解锁图像
			bmp.UnlockBits(bmData);

			// 把图像写入流
			if (Win32.AVIStreamWrite(streamCompressed, position, 1, buf,
				stride * height, 0, IntPtr.Zero, IntPtr.Zero) != 0)
				throw new ApplicationException("添加帧失败！");

			position++;
		}
	}
}
