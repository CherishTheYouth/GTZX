using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Helper
{
    /// <summary>
    /// 加密类，加密方式为MD5
    /// </summary>
    public static class EncryptHelper
    {
        // DES密钥，长度至少为8且不能为中文
        private const string DesrgbKey = "ox$4^&3H";
        private const string DesRgbIv = "&*56YMdx";

        /// <summary>
        /// MD5加密字符串（加密不可逆）
        /// </summary>
        /// <param name="targetString">目标字符串</param>
        /// <returns>加密后的字符串</returns>
        public static string Md5Encrypt(string targetString)
        {
            var sb = new StringBuilder();
            var md5 = MD5.Create();
            var bts = md5.ComputeHash(Encoding.Unicode.GetBytes(targetString));

            foreach (var bt in bts)
            {
                sb.Append(bt);
            }
            return sb.ToString();
        }

        /// <summary>
        /// DES加密字符串
        /// </summary>
        /// <param name="targetString">目标字符串</param>
        /// <returns>加密后的字符串</returns>
        public static string DesEncrypt(string targetString)
        {
            try
            {
                var rgbKey = Encoding.UTF8.GetBytes(DesrgbKey);
                var rgbIv = Encoding.UTF8.GetBytes(DesRgbIv);
                var inputByte = Encoding.UTF8.GetBytes(targetString);
                var serviceProvider = new DESCryptoServiceProvider();
                var ms = new MemoryStream();
                var cs = new CryptoStream(ms, serviceProvider.CreateEncryptor(rgbKey, rgbIv), CryptoStreamMode.Write);
                cs.Write(inputByte, 0, inputByte.Length);
                cs.FlushFinalBlock();
                return Convert.ToBase64String(ms.ToArray());
            }
            catch
            {
                return targetString;
            }
        }

        /// <summary>
        /// DES解密字符串
        /// </summary>
        /// <param name="targetString">目标字符串</param>
        /// <returns>解密后的字符串</returns>
        public static string DesDecrypt(string targetString)
        {
            try
            {
                var rgbKey = Encoding.UTF8.GetBytes(DesrgbKey);
                var rgbIv = Encoding.UTF8.GetBytes(DesRgbIv);
                var inputByte = Convert.FromBase64String(targetString);
                var serviceProvider = new DESCryptoServiceProvider();
                var ms = new MemoryStream();
                var cs = new CryptoStream(ms, serviceProvider.CreateDecryptor(rgbKey, rgbIv), CryptoStreamMode.Write);
                cs.Write(inputByte, 0, inputByte.Length);
                cs.FlushFinalBlock();
                return Encoding.UTF8.GetString(ms.ToArray());
            }
            catch
            {
                return targetString;
            }
        }

        /// <summary>
        /// Js翻译版DES加密
        /// </summary>
        /// <param name="targetString"></param>
        /// <returns></returns>
        public static string JsEncrypt(string targetString)
        {
            try
            {
                return StringToHex(Des(targetString, DesrgbKey, DesRgbIv, true));
            }
            catch
            {
                return targetString;
            }
        }

        /// <summary>
        /// Js翻译版DES解密
        /// </summary>
        /// <param name="targetString"></param>
        /// <returns></returns>
        public static string JsDecrypt(string targetString)
        {
            try
            {
                return Des(HexToString(targetString), DesrgbKey, DesRgbIv, false);
            }
            catch
            {
                return targetString;
            }
        }

        private static string Des(string strMessage, string key, string strIv, bool isEncrypt, int mode = 0)
        {
            var spfunction1 = new[]
            {
                0x1010400, 0, 0x10000, 0x1010404, 0x1010004, 0x10404, 0x4, 0x10000, 0x400, 0x1010400, 0x1010404, 0x400,
                0x1000404, 0x1010004, 0x1000000, 0x4, 0x404, 0x1000400, 0x1000400, 0x10400, 0x10400, 0x1010000,
                0x1010000, 0x1000404, 0x10004, 0x1000004, 0x1000004, 0x10004, 0, 0x404, 0x10404, 0x1000000, 0x10000,
                0x1010404, 0x4, 0x1010000, 0x1010400, 0x1000000, 0x1000000, 0x400, 0x1010004, 0x10000, 0x10400,
                0x1000004, 0x400, 0x4, 0x1000404, 0x10404, 0x1010404, 0x10004, 0x1010000, 0x1000404, 0x1000004, 0x404,
                0x10404, 0x1010400, 0x404, 0x1000400, 0x1000400, 0, 0x10004, 0x10400, 0, 0x1010004
            };
            var spfunction2 = new[]
            {
                -0x7fef7fe0, -0x7fff8000, 0x8000, 0x108020, 0x100000, 0x20, -0x7fefffe0, -0x7fff7fe0, -0x7fffffe0,
                -0x7fef7fe0, -0x7fef8000, -0x8000000, -0x7fff8000, 0x100000, 0x20, -0x7fefffe0, 0x108000, 0x100020,
                -0x7fff7fe0, 0, -0x8000000, 0x8000, 0x108020, -0x7ff00000, 0x100020, -0x7fffffe0, 0, 0x108000, 0x8020,
                -0x7fef8000, -0x7ff00000, 0x8020, 0, 0x108020, -0x7fefffe0, 0x100000, -0x7fff7fe0, -0x7ff00000,
                -0x7fef8000, 0x8000, -0x7ff00000, -0x7fff8000, 0x20, -0x7fef7fe0, 0x108020, 0x20, 0x8000, -0x8000000,
                0x8020, -0x7fef8000, 0x100000, -0x7fffffe0, 0x100020, -0x7fff7fe0, -0x7fffffe0, 0x100020, 0x108000, 0,
                -0x7fff8000, 0x8020, -0x8000000, -0x7fefffe0, -0x7fef7fe0, 0x108000
            };
            var spfunction3 = new[]
            {
                0x208, 0x8020200, 0, 0x8020008, 0x8000200, 0, 0x20208, 0x8000200, 0x20008, 0x8000008, 0x8000008, 0x20000,
                0x8020208, 0x20008, 0x8020000, 0x208, 0x8000000, 0x8, 0x8020200, 0x200, 0x20200, 0x8020000, 0x8020008,
                0x20208, 0x8000208, 0x20200, 0x20000, 0x8000208, 0x8, 0x8020208, 0x200, 0x8000000, 0x8020200, 0x8000000,
                0x20008, 0x208, 0x20000, 0x8020200, 0x8000200, 0, 0x200, 0x20008, 0x8020208, 0x8000200, 0x8000008, 0x200,
                0, 0x8020008, 0x8000208, 0x20000, 0x8000000, 0x8020208, 0x8, 0x20208, 0x20200, 0x8000008, 0x8020000,
                0x8000208, 0x208, 0x8020000, 0x20208, 0x8, 0x8020008, 0x20200
            };
            var spfunction4 = new[]
            {
                0x802001, 0x2081, 0x2081, 0x80, 0x802080, 0x800081, 0x800001, 0x2001, 0, 0x802000, 0x802000, 0x802081,
                0x81,
                0, 0x800080, 0x800001, 0x1, 0x2000, 0x800000, 0x802001, 0x80, 0x800000, 0x2001, 0x2080, 0x800081, 0x1,
                0x2080, 0x800080, 0x2000, 0x802080, 0x802081, 0x81, 0x800080, 0x800001, 0x802000, 0x802081, 0x81, 0, 0,
                0x802000, 0x2080, 0x800080, 0x800081, 0x1, 0x802001, 0x2081, 0x2081, 0x80, 0x802081, 0x81, 0x1, 0x2000,
                0x800001, 0x2001, 0x802080, 0x800081, 0x2001, 0x2080, 0x800000, 0x802001, 0x80, 0x800000, 0x2000,
                0x802080
            };
            var spfunction5 = new[]
            {
                0x100, 0x2080100, 0x2080000, 0x42000100, 0x80000, 0x100, 0x40000000, 0x2080000, 0x40080100, 0x80000,
                0x2000100, 0x40080100, 0x42000100, 0x42080000, 0x80100, 0x40000000, 0x2000000, 0x40080000, 0x40080000, 0,
                0x40000100, 0x42080100, 0x42080100, 0x2000100, 0x42080000, 0x40000100, 0, 0x42000000, 0x2080100,
                0x2000000, 0x42000000, 0x80100, 0x80000, 0x42000100, 0x100, 0x2000000, 0x40000000, 0x2080000, 0x42000100,
                0x40080100, 0x2000100, 0x40000000, 0x42080000, 0x2080100, 0x40080100, 0x100, 0x2000000, 0x42080000,
                0x42080100, 0x80100, 0x42000000, 0x42080100, 0x2080000, 0, 0x40080000, 0x42000000, 0x80100, 0x2000100,
                0x40000100, 0x80000, 0, 0x40080000, 0x2080100, 0x40000100
            };
            var spfunction6 = new[]
            {
                0x20000010, 0x20400000, 0x4000, 0x20404010, 0x20400000, 0x10, 0x20404010, 0x400000, 0x20004000, 0x404010,
                0x400000, 0x20000010, 0x400010, 0x20004000, 0x20000000, 0x4010, 0, 0x400010, 0x20004010, 0x4000,
                0x404000, 0x20004010, 0x10, 0x20400010, 0x20400010, 0, 0x404010, 0x20404000, 0x4010, 0x404000,
                0x20404000, 0x20000000, 0x20004000, 0x10, 0x20400010, 0x404000, 0x20404010, 0x400000, 0x4010, 0x20000010,
                0x400000, 0x20004000, 0x20000000, 0x4010, 0x20000010, 0x20404010, 0x404000, 0x20400000, 0x404010,
                0x20404000, 0, 0x20400010, 0x10, 0x4000, 0x20400000, 0x404010, 0x4000, 0x400010, 0x20004010, 0,
                0x20404000, 0x20000000, 0x400010, 0x20004010
            };
            var spfunction7 = new[]
            {
                0x200000, 0x4200002, 0x4000802, 0, 0x800, 0x4000802, 0x200802, 0x4200800, 0x4200802, 0x200000, 0,
                0x4000002,
                0x2, 0x4000000, 0x4200002, 0x802, 0x4000800, 0x200802, 0x200002, 0x4000800, 0x4000002, 0x4200000,
                0x4200800, 0x200002, 0x4200000, 0x800, 0x802, 0x4200802, 0x200800, 0x2, 0x4000000, 0x200800, 0x4000000,
                0x200800, 0x200000, 0x4000802, 0x4000802, 0x4200002, 0x4200002, 0x2, 0x200002, 0x4000000, 0x4000800,
                0x200000, 0x4200800, 0x802, 0x200802, 0x4200800, 0x802, 0x4000002, 0x4200802, 0x4200000, 0x200800, 0,
                0x2, 0x4200802, 0, 0x200802, 0x4200000, 0x800, 0x4000002, 0x4000800, 0x800, 0x200002
            };
            var spfunction8 = new[]
            {
                0x10001040, 0x1000, 0x40000, 0x10041040, 0x10000000, 0x10001040, 0x40, 0x10000000, 0x40040, 0x10040000,
                0x10041040, 0x41000, 0x10041000, 0x41040, 0x1000, 0x40, 0x10040000, 0x10000040, 0x10001000, 0x1040,
                0x41000, 0x40040, 0x10040040, 0x10041000, 0x1040, 0, 0, 0x10040040, 0x10000040, 0x10001000, 0x41040,
                0x40000, 0x41040, 0x40000, 0x10041000, 0x1000, 0x40, 0x10040040, 0x1000, 0x41040, 0x10001000, 0x40,
                0x10000040, 0x10040000, 0x10040040, 0x10000000, 0x40000, 0x10001040, 0, 0x10041040, 0x40040, 0x10000040,
                0x10040000, 0x10001000, 0x10001040, 0, 0x10041040, 0x41000, 0x41000, 0x1040, 0x1040, 0x40040, 0x10000000,
                0x10041000
            };

            var keys = CreateEncryptKey(key);

            int m = 0, i;
            int[] looping;
            int cbcleft = 0, cbcleft2 = 0, cbcright = 0, cbcright2 = 0;
            var len = strMessage.Length;
            var chunk = 0;
            var iterations = (keys.Length == 32) ? 3 : 9;
            if (iterations == 3)
            {
                looping = isEncrypt ? new[] { 0, 32, 2 } : new[] { 30, -2, -2 };
            }
            else
            {
                looping = isEncrypt
                    ? new[] { 0, 32, 2, 62, 30, -2, 64, 96, 2 }
                    : new[] { 94, 62, -2, 32, 64, 2, 30, -2, -2 };

            }

            var result = new StringBuilder();

            var tempresult = new StringBuilder();

            if (mode == 1)
            {
                var ivLen = strIv.Length;

                var civ = strIv.ToCharArray();

                var iv = new int[ivLen + 8];

                for (i = 0; i < ivLen; i++)
                {
                    iv[i] = Convert.ToInt32(civ[i]);
                }

                for (i = ivLen; i < (ivLen + 8); ++i)
                {
                    iv[i] = 0;
                }

                cbcleft = (iv[m++] << 24) | (iv[m++] << 16) | (iv[m++] << 8) | iv[m++];
                cbcright = (iv[m++] << 24) | (iv[m++] << 16) | (iv[m++] << 8) | iv[m];
                m = 0;
            }

            while (m < len)
            {
                var message = new int[len + 8];
                var cm = strMessage.ToCharArray();
                for (i = 0; i < len; ++i)
                {
                    message[i] = Convert.ToInt32(cm[i]);
                }

                int left;
                int right;
                if (isEncrypt)
                {
                    left = (message[m++] << 16) | message[m++];
                    right = (message[m++] << 16) | message[m++];
                }
                else
                {
                    left = (message[m++] << 24) | (message[m++] << 16) | (message[m++] << 8) | message[m++];
                    right = (message[m++] << 24) | (message[m++] << 16) | (message[m++] << 8) | message[m++];
                }

                if (mode == 1)
                {
                    if (isEncrypt)
                    {
                        left ^= cbcleft;
                        right ^= cbcright;
                    }
                    else
                    {
                        cbcleft2 = cbcleft;
                        cbcright2 = cbcright;
                        cbcleft = left;
                        cbcright = right;
                    }
                }

                var temp = (MoveByte(left, 4) ^ right) & 0x0f0f0f0f;
                right ^= temp;
                left ^= (temp << 4);
                temp = (MoveByte(left, 16) ^ right) & 0x0000ffff;
                right ^= temp;
                left ^= (temp << 16);
                temp = (MoveByte(right, 2) ^ left) & 0x33333333;
                left ^= temp;
                right ^= (temp << 2);
                temp = (MoveByte(right, 8) ^ left) & 0x00ff00ff;
                left ^= temp;
                right ^= (temp << 8);
                temp = (MoveByte(left, 1) ^ right) & 0x55555555;
                right ^= temp;
                left ^= (temp << 1);
                left = ((left << 1) | MoveByte(left, 31));
                right = ((right << 1) | MoveByte(right, 31));

                int j;
                for (j = 0; j < iterations; j += 3)
                {
                    var endloop = looping[j + 1];
                    var loopinc = looping[j + 2];

                    for (i = looping[j]; i != endloop; i += loopinc)
                    {
                        var right1 = right ^ keys[i];
                        var right2 = (MoveByte(right, 4) | (right << 28)) ^ keys[i + 1];
                        temp = left;
                        left = right;
                        right = temp ^
                                (spfunction2[MoveByte(right1, 24) & 0x3f] | spfunction4[MoveByte(right1, 16) & 0x3f] |
                                 spfunction6[MoveByte(right1, 8) & 0x3f] | spfunction8[right1 & 0x3f] |
                                 spfunction1[MoveByte(right2, 24) & 0x3f] | spfunction3[MoveByte(right2, 16) & 0x3f] |
                                 spfunction5[MoveByte(right2, 8) & 0x3f] | spfunction7[right2 & 0x3f]);
                    }

                    temp = left;
                    left = right;
                    right = temp;
                }

                left = (MoveByte(left, 1) | (left << 31));
                right = (MoveByte(right, 1) | (right << 31));
                temp = (MoveByte(left, 1) ^ right) & 0x55555555;
                right ^= temp;
                left ^= (temp << 1);
                temp = (MoveByte(right, 8) ^ left) & 0x00ff00ff;
                left ^= temp;
                right ^= (temp << 8);
                temp = (MoveByte(right, 2) ^ left) & 0x33333333;
                left ^= temp;
                right ^= (temp << 2);
                temp = (MoveByte(left, 16) ^ right) & 0x0000ffff;
                right ^= temp;
                left ^= (temp << 16);
                temp = (MoveByte(left, 4) ^ right) & 0x0f0f0f0f;
                right ^= temp;
                left ^= (temp << 4);

                if (mode == 1)
                {
                    if (isEncrypt)
                    {
                        cbcleft = left;
                        cbcright = right;
                    }
                    else
                    {
                        left ^= cbcleft2;
                        right ^= cbcright2;
                    }
                }

                if (isEncrypt)
                {
                    tempresult.Append(Convert.ToChar(MoveByte(left, 24)));
                    tempresult.Append(Convert.ToChar((MoveByte(left, 16) & 0xff)));
                    tempresult.Append(Convert.ToChar((MoveByte(left, 8) & 0xff)));
                    tempresult.Append(Convert.ToChar((left & 0xff)));
                    tempresult.Append(Convert.ToChar(MoveByte(right, 24)));
                    tempresult.Append(Convert.ToChar((MoveByte(right, 16) & 0xff)));
                    tempresult.Append(Convert.ToChar((MoveByte(right, 8) & 0xff)));
                    tempresult.Append(Convert.ToChar((right & 0xff)));
                }
                else
                {
                    tempresult.Append(Convert.ToChar(((MoveByte(left, 16) & 0xffff))));
                    tempresult.Append(Convert.ToChar((left & 0xffff)));
                    tempresult.Append(Convert.ToChar((MoveByte(right, 16) & 0xffff)));
                    tempresult.Append(Convert.ToChar((right & 0xffff)));
                }

                if (isEncrypt)
                {
                    chunk += 16;
                }
                else
                {
                    chunk += 8;
                }

                if (chunk == 512)
                {
                    result.Append(tempresult);
                    tempresult.Remove(0, tempresult.Length);
                    chunk = 0;
                }
            }

            var tempString = string.Concat(result.ToString(), tempresult.ToString());

            if (isEncrypt) return tempString;

            // 解密时移除空字符
            var emptyChar = Convert.ToChar(0);
            var sb = new StringBuilder(tempString);
            for (var j = sb.Length - 1; j >= 0; j--)
            {
                if (sb[j] == emptyChar)
                {
                    sb.Remove(j, 1);
                }
                else
                {
                    break;
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// 密钥生成函数
        /// </summary>
        /// <param name="strKey"></param>
        /// <returns></returns>
        private static int[] CreateEncryptKey(string strKey)
        {
            var pc2Bytes0 = new[]
            {
                0, 0x4, 0x20000000, 0x20000004, 0x10000, 0x10004, 0x20010000, 0x20010004, 0x200, 0x204, 0x20000200,
                0x20000204, 0x10200, 0x10204, 0x20010200, 0x20010204
            };

            var pc2Bytes1 = new[]
            {
                0, 0x1, 0x100000, 0x100001, 0x4000000, 0x4000001, 0x4100000, 0x4100001, 0x100, 0x101, 0x100100, 0x100101,
                0x4000100, 0x4000101, 0x4100100, 0x4100101
            };

            var pc2Bytes2 = new[]
            {
                0, 0x8, 0x800, 0x808, 0x1000000, 0x1000008, 0x1000800, 0x1000808, 0, 0x8, 0x800, 0x808, 0x1000000,
                0x1000008, 0x1000800, 0x1000808
            };

            var pc2Bytes3 = new[]
            {
                0, 0x200000, 0x8000000, 0x8200000, 0x2000, 0x202000, 0x8002000, 0x8202000, 0x20000, 0x220000, 0x8020000,
                0x8220000, 0x22000, 0x222000, 0x8022000, 0x8222000
            };

            var pc2Bytes4 = new[]
            {
                0, 0x40000, 0x10, 0x40010, 0, 0x40000, 0x10, 0x40010, 0x1000, 0x41000, 0x1010, 0x41010, 0x1000, 0x41000,
                0x1010, 0x41010
            };

            var pc2Bytes5 = new[]
            {
                0, 0x400, 0x20, 0x420, 0, 0x400, 0x20, 0x420, 0x2000000, 0x2000400, 0x2000020, 0x2000420, 0x2000000,
                0x2000400, 0x2000020, 0x2000420
            };

            var pc2Bytes6 = new[]
            {
                0, 0x10000000, 0x80000, 0x10080000, 0x2, 0x10000002, 0x80002, 0x10080002, 0, 0x10000000, 0x80000,
                0x10080000, 0x2, 0x10000002, 0x80002, 0x10080002
            };

            var pc2Bytes7 = new[]
            {
                0, 0x10000, 0x800, 0x10800, 0x20000000, 0x20010000, 0x20000800, 0x20010800, 0x20000, 0x30000, 0x20800,
                0x30800, 0x20020000, 0x20030000, 0x20020800, 0x20030800
            };

            var pc2Bytes8 = new[]
            {
                0, 0x40000, 0, 0x40000, 0x2, 0x40002, 0x2, 0x40002, 0x2000000, 0x2040000, 0x2000000, 0x2040000,
                0x2000002,
                0x2040002, 0x2000002, 0x2040002
            };

            var pc2Bytes9 = new[]
            {
                0, 0x10000000, 0x8, 0x10000008, 0, 0x10000000, 0x8, 0x10000008, 0x400, 0x10000400, 0x408, 0x10000408,
                0x400,
                0x10000400, 0x408, 0x10000408
            };

            var pc2Bytes10 = new[]
            {
                0, 0x20, 0, 0x20, 0x100000, 0x100020, 0x100000, 0x100020, 0x2000, 0x2020, 0x2000, 0x2020, 0x102000,
                0x102020, 0x102000, 0x102020
            };

            var pc2Bytes11 = new[]
            {
                0, 0x1000000, 0x200, 0x1000200, 0x200000, 0x1200000, 0x200200, 0x1200200, 0x4000000, 0x5000000,
                0x4000200,
                0x5000200, 0x4200000, 0x5200000, 0x4200200, 0x5200200
            };

            var pc2Bytes12 = new[]
            {
                0, 0x1000, 0x8000000, 0x8001000, 0x80000, 0x81000, 0x8080000, 0x8081000, 0x10, 0x1010, 0x8000010,
                0x8001010,
                0x80010, 0x81010, 0x8080010, 0x8081010
            };

            var pc2Bytes13 = new[] { 0, 0x4, 0x100, 0x104, 0, 0x4, 0x100, 0x104, 0x1, 0x5, 0x101, 0x105, 0x1, 0x5, 0x101, 0x105 };

            var iterations = strKey.Length >= 24 ? 3 : 1;

            var keys = new int[32 * iterations];
            var shifts = new[] { 0, 0, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 0 };
            int m = 0, n = 0;

            var ckey = strKey.ToCharArray();

            var strLen = strKey.Length;
            var keyLen = strLen + iterations * 8;
            var key = new int[keyLen];

            for (var i = 0; i < strLen; ++i)
            {
                key[i] = Convert.ToInt32(ckey[i]);
            }

            for (var i = strLen; i < keyLen; ++i)
            {
                key[i] = 0;
            }

            for (var j = 0; j < iterations; j++)
            {
                var left = (key[m++] << 24) | (key[m++] << 16) | (key[m++] << 8) | key[m++];
                var right = (key[m++] << 24) | (key[m++] << 16) | (key[m++] << 8) | key[m++];
                var temp = (MoveByte(left, 4) ^ right) & 0x0f0f0f0f;

                right ^= temp;
                left ^= (temp << 4);
                temp = (MoveByte(right, -16) ^ left) & 0x0000ffff;
                left ^= temp;
                right ^= (temp << -16);
                temp = (MoveByte(left, 2) ^ right) & 0x33333333;
                right ^= temp;
                left ^= (temp << 2);
                temp = (MoveByte(right, -16) ^ left) & 0x0000ffff;
                left ^= temp;
                right ^= (temp << -16);
                temp = (MoveByte(left, 1) ^ right) & 0x55555555;
                right ^= temp;
                left ^= (temp << 1);
                temp = (MoveByte(right, 8) ^ left) & 0x00ff00ff;
                left ^= temp;
                right ^= (temp << 8);
                temp = (MoveByte(left, 1) ^ right) & 0x55555555;
                right ^= temp;
                left ^= (temp << 1);
                temp = (left << 8) | (MoveByte(right, 20) & 0x000000f0);
                left = (right << 24) | ((right << 8) & 0xff0000) | (MoveByte(right, 8) & 0xff00) |
                       (MoveByte(right, 24) & 0xf0);
                right = temp;

                var shiftLen = shifts.Length;
                for (var i = 0; i < shiftLen; i++)
                {
                    if (shifts[i] == 1)
                    {
                        left = (left << 2) | MoveByte(left, 26);
                        right = (right << 2) | MoveByte(right, 26);
                    }
                    else
                    {
                        left = (left << 1) | MoveByte(left, 27);
                        right = (right << 1) | MoveByte(right, 27);
                    }
                    left &= -0xf;
                    right &= -0xf;
                    var lefttemp = pc2Bytes0[MoveByte(left, 28)] | pc2Bytes1[MoveByte(left, 24) & 0xf] |
                                   pc2Bytes2[MoveByte(left, 20) & 0xf] | pc2Bytes3[MoveByte(left, 16) & 0xf] |
                                   pc2Bytes4[MoveByte(left, 12) & 0xf] | pc2Bytes5[MoveByte(left, 8) & 0xf] |
                                   pc2Bytes6[MoveByte(left, 4) & 0xf];
                    var righttemp = pc2Bytes7[MoveByte(right, 28)] | pc2Bytes8[MoveByte(right, 24) & 0xf] |
                                    pc2Bytes9[MoveByte(right, 20) & 0xf] | pc2Bytes10[MoveByte(right, 16) & 0xf] |
                                    pc2Bytes11[MoveByte(right, 12) & 0xf] | pc2Bytes12[MoveByte(right, 8) & 0xf] |
                                    pc2Bytes13[MoveByte(right, 4) & 0xf];
                    temp = (MoveByte(righttemp, 16) ^ lefttemp) & 0x0000ffff;
                    keys[n++] = lefttemp ^ temp;
                    keys[n++] = righttemp ^ (temp << 16);
                }
            }

            return keys;
        }

        /// <summary>
        /// 实现无符号右移,相当于javascript中的>>>运算符
        /// </summary>
        /// <param name="val"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        private static int MoveByte(int val, int pos)
        {
            pos = pos % 32;
            if (pos < 0) pos += 32;

            //取得二进制字符串
            var strBit = Convert.ToString(val, 2);

            return strBit.Length <= pos ? 0 : Convert.ToInt32(strBit.Substring(0, strBit.Length - pos), 2);
        }

        /// <summary>
        /// 将普通的字符串转换成16进制的字符串
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private static string StringToHex(string s)
        {
            var sb = new StringBuilder();
            var hexs = new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };

            var len = s.Length;
            var cs = s.ToCharArray();
            for (var i = 0; i < len; ++i)
            {
                sb.Append(hexs[cs[i] >> 4]);
                sb.Append(hexs[cs[i] & 0xf]);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 将16进制的字符串转换成普通的字符串
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private static string HexToString(string s)
        {
            var sb = new StringBuilder();

            var array = s.ToArray();
            for (var i = 0; i < array.Length; i += 2)
            {
                var c = Convert.ToChar(Convert.ToInt16(string.Concat(array[i], array[i + 1]), 16));
                sb.Append(c);
            }

            return sb.ToString();
        }
    }
}