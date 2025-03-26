using ChildTracking.Repositories;
using ChildTracking.Repositories.Models;
using ChildTracking.Services;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ChildTracking.SoapService.HoangVM.SoapServices
{
    [DataContract]
    public class CreateChildRequest
    {
        [DataMember]
        public Child ChildData { get; set; }
    }

    [DataContract]
    public class CreateChildResponse
    {
        [DataMember]
        public bool Success { get; set; }

        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public int ChildId { get; set; }
    }

    [DataContract]
    public class UpdateChildRequest
    {
        [DataMember]
        public Child ChildData { get; set; }
    }

    [DataContract]
    public class UpdateChildResponse
    {
        [DataMember]
        public bool Success { get; set; }

        [DataMember]
        public string Message { get; set; }
    }

    [DataContract]
    public class DeleteChildRequest
    {
        [DataMember]
        public Guid ChildId { get; set; }
    }

    [DataContract]
    public class DeleteChildResponse
    {
        [DataMember]
        public bool Success { get; set; }

        [DataMember]
        public string Message { get; set; }
    }

    [DataContract]
    public class GetChildByIdRequest
    {
        [DataMember]
        public Guid ChildId { get; set; }
    }

    [DataContract]
    public class GetAllChildrenResponse
    {
        [DataMember]
        public List<Child> Children { get; set; }
    }

    [ServiceContract]
    public interface IChildSoapService
    {
        [OperationContract]
        Task<GetAllChildrenResponse> GetAll();

        [OperationContract]
        Task<Child> GetById(GetChildByIdRequest request);

        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        Task<CreateChildResponse> Create(CreateChildRequest request);

        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        Task<UpdateChildResponse> Update(UpdateChildRequest request);

        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        Task<DeleteChildResponse> Delete(DeleteChildRequest request);
    }

    [DataContract]
    public class ServiceFault
    {
        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public string Details { get; set; }
    }

    public class ChildSoapService : IChildSoapService
    {
        private readonly IChildService _childService;
        private readonly GrowthRecordService _growthRecordService;
        private readonly JsonSerializerOptions _jsonOptions;
        public ChildSoapService(IChildService childService, GrowthRecordService growthRecordService)
        {
            _childService = childService;
            _growthRecordService = growthRecordService;
            _jsonOptions = new JsonSerializerOptions()
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
        }

        public async Task<CreateChildResponse> Create(CreateChildRequest request)
        {
            try
            {
                if (request?.ChildData == null)
                {
                    throw new FaultException<ServiceFault>(
                        new ServiceFault
                        {
                            Message = "Invalid request",
                            Details = "Child data cannot be null"
                        },
                        new FaultReason("Invalid request data"));
                }

                var result = await _childService.Create(request.ChildData);

                return new CreateChildResponse
                {
                    Success = result > 0,
                    Message = result > 0 ? "Child created successfully" : "Failed to create child",
                    ChildId = result
                };
            }
            catch (Exception ex)
            {
                throw new FaultException<ServiceFault>(
                    new ServiceFault
                    {
                        Message = "Error creating child",
                        Details = ex.Message
                    },
                    new FaultReason("An error occurred while processing your request"));
            }
        }

        public async Task<DeleteChildResponse> Delete(DeleteChildRequest request)
        {
            try
            {
                if (request?.ChildId == Guid.Empty)
                {
                    throw new FaultException<ServiceFault>(
                        new ServiceFault
                        {
                            Message = "Invalid request",
                            Details = "Child ID cannot be empty"
                        },
                        new FaultReason("Invalid request data"));
                }

                var result = await _childService.Delete(request.ChildId);

                return new DeleteChildResponse
                {
                    Success = result,
                    Message = result ? "Child deleted successfully" : "Failed to delete child"
                };
            }
            catch (Exception ex)
            {
                throw new FaultException<ServiceFault>(
                    new ServiceFault
                    {
                        Message = "Error deleting child",
                        Details = ex.Message
                    },
                    new FaultReason("An error occurred while processing your request"));
            }
        }

        public async Task<GetAllChildrenResponse> GetAll()
        {
            try
            {
                var items = await _childService.GetAll();
 
                var itemsString = JsonSerializer.Serialize(items, _jsonOptions);
                var result = JsonSerializer.Deserialize<List<Child>>(itemsString, _jsonOptions);

                return new GetAllChildrenResponse { Children = result ?? new List<Child>() };
            }
            catch (Exception ex)
            {
                throw new FaultException<ServiceFault>(
                    new ServiceFault
                    {
                        Message = "Error getting all children",
                        Details = ex.Message
                    },
                    new FaultReason("An error occurred while processing your request"));
            }
        }

        public async Task<Child> GetById(GetChildByIdRequest request)
        {
            try
            {
                if (request?.ChildId == Guid.Empty)
                {
                    throw new FaultException<ServiceFault>(
                        new ServiceFault
                        {
                            Message = "Invalid request",
                            Details = "Child ID cannot be empty"
                        },
                        new FaultReason("Invalid request data"));
                }

                var item = await _childService.GetById(request.ChildId);

                if (item == null)
                {
                    throw new FaultException<ServiceFault>(
                        new ServiceFault
                        {
                            Message = "Child not found",
                            Details = $"No child found with ID {request.ChildId}"
                        },
                        new FaultReason("Child not found"));
                }


                var itemString = JsonSerializer.Serialize(item, _jsonOptions);
                var result = JsonSerializer.Deserialize<Child>(itemString, _jsonOptions);

                return result;
            }
            catch (FaultException)
            {
                throw; // Re-throw fault exceptions
            }
            catch (Exception ex)
            {
                throw new FaultException<ServiceFault>(
                    new ServiceFault
                    {
                        Message = "Error getting child",
                        Details = ex.Message
                    },
                    new FaultReason("An error occurred while processing your request"));
            }
        }

        public async Task<UpdateChildResponse> Update(UpdateChildRequest request)
        {
            try
            {
                if (request?.ChildData == null)
                {
                    throw new FaultException<ServiceFault>(
                        new ServiceFault
                        {
                            Message = "Invalid request",
                            Details = "Child data cannot be null"
                        },
                        new FaultReason("Invalid request data"));
                }

                var result = await _childService.Update(request.ChildData);

                return new UpdateChildResponse
                {
                    Success = result > 0,
                    Message = result > 0 ? "Child updated successfully" : "Failed to update child"
                };
            }
            catch (Exception ex)
            {
                throw new FaultException<ServiceFault>(
                    new ServiceFault
                    {
                        Message = "Error updating child",
                        Details = ex.Message
                    },
                    new FaultReason("An error occurred while processing your request"));
            }
        }
    }
}