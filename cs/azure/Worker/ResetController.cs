﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using VowpalWabbit.Azure.Trainer;

namespace VowpalWabbit.Azure.Worker
{
    public sealed class ResetController : OnlineTrainerController
    {
        public ResetController(LearnEventProcessorHost trainProcessorFactory)
            : base(trainProcessorFactory)
        {
        }

        public async Task<HttpResponseMessage> Get()
        {
            if (!this.TryAuthorize())
                return this.Request.CreateResponse(HttpStatusCode.Unauthorized);

            try
            {
                await this.trainProcessorHost.ResetModelAsync();

                return this.Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                this.telemetry.TrackException(ex);
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        public async Task<HttpResponseMessage> Post()
        {
            if (!this.TryAuthorize())
                return this.Request.CreateResponse(HttpStatusCode.Unauthorized);

            try
            {
                OnlineTrainerState state = null;
                var body = await Request.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(body))
                    state = JsonConvert.DeserializeObject<OnlineTrainerState>(body);

                await this.trainProcessorHost.ResetModelAsync(state);

                return this.Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                this.telemetry.TrackException(ex);
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
