﻿using System.IO;
using System.Linq;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Azure.Test.HttpRecorder;
using Microsoft.Rest.ClientRuntime.Azure.TestFramework;
using Xunit;

namespace ComputerVisionSDK.Tests
{
    public class VisionTagTests : BaseTests
    {
        [Fact(Skip = "https://github.com/Azure/azure-sdk-for-net/issues/6214")]
        public void TagImageInStreamTest()
        {
            using (MockContext context = MockContext.Start(this.GetType()))
            {
                HttpMockServer.Initialize(this.GetType(), "TagImageInStreamTest");

                const string Chinese = "zh";

                using (IComputerVisionClient client = GetComputerVisionClient(HttpMockServer.CreateInstance()))
                using (FileStream stream = new FileStream(GetTestImagePath("house.jpg"), FileMode.Open))
                {
                    TagResult result = client.TagImageInStreamAsync(stream, Chinese).Result;

                    Assert.Equal(
                        new string[] { "草", "户外", "天空", "屋子", "建筑", "绿色", "草坪", "住宅", "绿色的", "家", "房子", "屋顶", "平房", "车库", "历史" },
                        result.Tags.Select(tag => tag.Name).ToArray());
                }
            }
        }

        [Fact(Skip = "https://github.com/Azure/azure-sdk-for-net/issues/6214")]
        public void TagImageTest()
        {
            using (MockContext context = MockContext.Start(this.GetType()))
            {
                HttpMockServer.Initialize(this.GetType(), "TagImageTest");

                string imageUrl = GetTestImageUrl("house.jpg");

                using (IComputerVisionClient client = GetComputerVisionClient(HttpMockServer.CreateInstance()))
                {
                    TagResult result = client.TagImageAsync(imageUrl).Result;

                    Assert.Equal(
                        new string[] { "grass", "outdoor", "sky", "house", "building", "green", "lawn", "residential", "grassy", "home", "roof", "bungalow", "garage" , "historic" },
                        result.Tags.Select(tag => tag.Name).ToArray());

                    // Confirm tags are in descending confidence order
                    var orignalConfidences = result.Tags.Select(tag => tag.Confidence).ToArray();
                    var sortedConfidences = orignalConfidences.OrderByDescending(c => c).ToArray();
                    Assert.Equal(sortedConfidences, orignalConfidences);
                }
            }
        }
    }
}
