
/*
用于生成唯一的、递增的Id。生成的规则如下：
1、生成的Id包含一个固定前缀值
2、为了生成尽可能多的不重复数字，所以使用int64来表示一个数字，其中：
0 000000000000000 0000000000000000000000000000 00000000000000000000
第一部分：1位，固定为0
第二部分：共PrefixBitCount位，表示固定前缀值。范围为[0, math.Pow(2, PrefixBitCount))
第三部分：共TimeBitCount位，表示当前时间距离基础时间的秒数。范围为[0, math.Pow(2, TimeBitCount))，以2019-1-1 00:00:00为基准则可以持续到2025-07-01 00:00:00
第四部分：共SeedBitCount位，表示自增种子。范围为[0, math.Pow(2, SeedBitCount))
3、总体而言，此规则支持每秒生成math.Pow(2, SeedBitCount)个不同的数字，并且在math.Pow(2, TimeBitCount)/60/60/24/365年的时间范围内有效
*/

using System;

namespace Framework
{
    /// <summary>
    /// Id生成助手类
    /// </summary>
    public class IdUtil
    {
        /// <summary>
        /// 前缀所占的位数
        /// </summary>
        public Int32 PrefixBitCount { get; set; }

        /// <summary>
        /// 最小的前缀值
        /// </summary>
        private Int64 MinPrefix { get; set; }

        /// <summary>
        /// 最大的前缀值
        /// </summary>
        private Int64 MaxPrefix { get; set; }

        /// <summary>
        /// 时间戳所占的位数
        /// </summary>
        public Int32 TimeBitCount { get; set; }

        /// <summary>
        /// 自增种子所占的位数
        /// </summary>
        public Int32 SeedBitCount { get; set; }

        /// <summary>
        /// 当前种子值
        /// </summary>
        private Int64 CurrSeed { get; set; }

        /// <summary>
        /// 最小的种子值
        /// </summary>
        private Int64 MinSeed { get; set; }

        /// <summary>
        /// 最大的种子值
        /// </summary>
        private Int64 MaxSeed { get; set; }

        /// <summary>
        /// 锁对象
        /// </summary>
        private Object LockObj { get; set; }

        /// <summary>
        /// 创建Id助手类对象（为了保证Id的唯一，需要保证生成的对象全局唯一）
        /// prefixBitCount + timeBitCount + seedBitCount <= 63
        /// </summary>
        /// <param name="prefixBitCount">表示id前缀的位数</param>
        /// <param name="timeBitCount">表示时间的位数</param>
        /// <param name="seedBitCount">表示自增种子的位数</param>
        public IdUtil(Int32 prefixBitCount, Int32 timeBitCount, Int32 seedBitCount)
        {
            // 之所以使用63位而不是64，是为了保证值为正数
            if (prefixBitCount + timeBitCount + seedBitCount > 63)
            {
                throw new ArgumentOutOfRangeException(String.Format("总位数{0}超过63位，请调整所有值的合理范围。", (prefixBitCount + timeBitCount + seedBitCount).ToString()));
            }

            this.PrefixBitCount = prefixBitCount;
            this.TimeBitCount = timeBitCount;
            this.SeedBitCount = seedBitCount;

            this.MinPrefix = 0;
            this.MaxPrefix = (Int64)System.Math.Pow(2, this.PrefixBitCount) - 1;
            this.CurrSeed = 0;
            this.MinSeed = 0;
            this.MaxSeed = (Int64)System.Math.Pow(2, this.SeedBitCount) - 1;
            Console.WriteLine(String.Format("Prefix:[{0},{1}], Time:{2} Year, Seed:[{3},{4}]", this.MinPrefix, this.MaxPrefix, (Int64)(System.Math.Pow(2, this.TimeBitCount) / 60 / 60 / 24 / 365), this.MinSeed, this.MaxSeed));
            this.LockObj = new Object();
        }

        private Int64 GetTimeStamp()
        {
            DateTime startTime = new DateTime(2019, 1, 1);
            DateTime currTime = DateTime.Now;
            return (Int64)System.Math.Round((currTime - startTime).TotalSeconds, MidpointRounding.AwayFromZero);
        }

        private Int64 GenerateNewSeed()
        {
            lock (this.LockObj)
            {
                if (this.CurrSeed >= this.MaxSeed)
                {
                    this.CurrSeed = this.MinSeed;
                }
                else
                {
                    this.CurrSeed += 1;
                }

                return this.CurrSeed;
            }
        }

        /// <summary>
        /// 生成新的Id
        /// </summary>
        /// <param name="prefix">Id的前缀值。取值范围必须可以用初始化时指定的前缀值的位数来表示，否则会抛出ArgumentOutOfRangeException</param>
        /// <returns>新的Id</returns>
        public Int64 GenerateNewId(Int64 prefix)
        {
            if (prefix < this.MinPrefix || prefix > this.MaxPrefix)
            {
                throw new ArgumentOutOfRangeException(String.Format("前缀的值溢出，有效范围为【{0},{1}】", this.MinPrefix.ToString(), this.MaxPrefix.ToString()));
            }

            Int64 stamp = this.GetTimeStamp();
            Int64 seed = this.GenerateNewSeed();

            return (prefix << (this.TimeBitCount + this.SeedBitCount)) | (stamp << this.SeedBitCount) | seed;
        }
    }
}