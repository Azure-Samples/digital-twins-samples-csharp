// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;

namespace Microsoft.Azure.DigitalTwins.Samples.Tests
{
    public static class FakeDigitalTwinsHttpClient
    {
        public static Models.Space Space = new Models.Space()
        {
            Name = "Space1_HttpClient",
            Id = new Guid("90000000-0000-0000-0000-000000000001").ToString(),
            Type = "Space1Type",
        };

        public static Models.Device Device = new Models.Device()
        {
            Name = "Device1_HttpClient",
            Id = new Guid("90000000-0000-0000-0000-000000000001").ToString(),
            HardwareId = "DeviceHardwareId1_HttpClient",
            SpaceId = Space.Id,
        };

        public static (HttpClient, FakeHttpHandler) Create(
            IEnumerable<Guid> postResponseGuids = null,
            IEnumerable<HttpResponseMessage> getResponses = null)
        {
            return FakeHttpHandler.CreateHttpClient(
                postResponses: postResponseGuids != null ? CreateGuidResponses(postResponseGuids) : null,
                getResponses: getResponses);
        }

        // Creates an httpClient that will respond with a space (this.Space or passed in space)
        public static (HttpClient, FakeHttpHandler) CreateWithSpace(
            IEnumerable<Guid> postResponseGuids,
            IEnumerable<HttpResponseMessage> getResponses = null,
            IEnumerable<HttpResponseMessage> patchResponses = null,
            Models.Space space = null)
        {
            postResponseGuids = postResponseGuids ?? Array.Empty<Guid>();
            getResponses = getResponses ?? Array.Empty<HttpResponseMessage>();
            patchResponses = patchResponses ?? Array.Empty<HttpResponseMessage>();
            space = space ?? Space;

            var getRootSpaceResponse = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(new [] { space })),
            };

            var getSensorsForResultsResponse = new [] { Responses.NotFound };

            return FakeHttpHandler.CreateHttpClient(
                postResponses: CreateGuidResponses(postResponseGuids),
                getResponses: new [] { getRootSpaceResponse }
                    .Concat(getResponses)
                    .Concat(getSensorsForResultsResponse),
                patchResponses: patchResponses);
        }

        // Creates an httpClient that will respond with a space and device
        // (this.Space and this.Device)
        public static (HttpClient, FakeHttpHandler) CreateWithDevice(
            IEnumerable<Guid> postResponseGuids,
            IEnumerable<HttpResponseMessage> getResponses = null)
        {
            getResponses = getResponses ?? Array.Empty<HttpResponseMessage>();

            var getDeviceResponse = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(Device)),
            };
            var getDevicesResponse = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(new [] { Device })),
            };
            return CreateWithSpace(
                postResponseGuids: postResponseGuids,
                getResponses: new [] { getDevicesResponse, getDeviceResponse }.Concat(getResponses) );
        }

        private static IEnumerable<HttpResponseMessage> CreateGuidResponses(IEnumerable<Guid> guids)
            => guids.Select(guid => new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent($"\"{guid.ToString()}\""),
                });
    }
}