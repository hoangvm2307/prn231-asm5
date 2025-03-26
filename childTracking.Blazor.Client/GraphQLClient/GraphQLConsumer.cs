using childTracking.Blazor.Client.Models;
using GraphQL;
using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace childTracking.Blazor.Client.GraphQLClient
{
    public class GraphQLConsumer
    {
        private static string APIEndPoint = "https://localhost:7229/graphql";
        private readonly GraphQLHttpClient _graphqlClient;

        public GraphQLConsumer()
        {
            _graphqlClient = new GraphQLHttpClient(
     new GraphQLHttpClientOptions { EndPoint = new Uri(APIEndPoint) },
     new NewtonsoftJsonSerializer());
            
        }

        public async Task<List<Child>> GetAllChildren()
        {
            try
            {
                var graphQLRequest = new GraphQLRequest
                {
                    Query = @"
                        query All {
                            all {
                                userId
                                fullName
                                dateOfBirth
                                gender
                                bloodType
                                medicalConditions
                                allergies
                                notes
                                createdAt
                                childId
                            }
                        }
                    ",
                    Variables = new { timestamp = DateTime.Now.Ticks }
                };

                _graphqlClient.HttpClient.DefaultRequestHeaders.CacheControl =
            new System.Net.Http.Headers.CacheControlHeaderValue { NoCache = true, NoStore = true };

                var response = await _graphqlClient.SendQueryAsync<dynamic>(graphQLRequest);

                if (response != null && response.Data != null && response.Data.all != null)
                {
                    // Chuyển đổi từ dynamic sang List<Child>
                    var childrenJson = JsonConvert.SerializeObject(response.Data.all);
                    return JsonConvert.DeserializeObject<List<Child>>(childrenJson);
                }

                return new List<Child>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy dữ liệu: {ex.Message}");
                return new List<Child>();
            }
        }



        public async Task<Child> GetChildById(Guid childId)
        {
            try
            {
                var graphQLRequest = new GraphQLRequest
                {
                    Query = @"
                    query GetChildById($childId: UUID!) {
                        childById(childId: $childId) {
                            childId
                            userId
                            fullName
                            dateOfBirth
                            gender
                            bloodType
                            medicalConditions
                            allergies
                            notes
                            createdAt
                            growthRecords {
                                recordId
                                childId
                                measurementDate
                                weight
                                height
                                bmi
                                headCircumference
                                notes
                                measuredBy
                                status
                                createdAt
                            }
                        }
                    }
                    ",
                    Variables = new
                    {
                        childId = childId.ToString()
                    }
                };

                var response = await _graphqlClient.SendQueryAsync<dynamic>(graphQLRequest);
                if (response != null && response.Data != null && response.Data.childById != null)
                {
                    // Chuyển đổi từ dynamic sang Child
                    var childJson = JsonConvert.SerializeObject(response.Data.childById);
                    return JsonConvert.DeserializeObject<Child>(childJson);
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy dữ liệu trẻ theo ID: {ex.Message}");
                return null;
            }
        }

        public async Task<Child> CreateChild(Child child)
        {
            if (child == null)
            {
                Console.WriteLine("Child object is null.");
                return null;
            }

            try
            {
                // Tạo GUID mới nếu chưa có
                if (child.ChildId == Guid.Empty)
                {
                    child.ChildId = Guid.NewGuid();
                }

                // Sửa lại truy vấn GraphQL để phù hợp với server API
                var graphQLRequest = new GraphQLRequest
                {
                    Query = @"
            mutation CreateChild($input: ChildInput!) {
                createChild(input: $input)
            }",
                    Variables = new
                    {
                        input = new
                        {
                            childId = child.ChildId.ToString(),
                            userId = child.UserId,
                            fullName = child.FullName,
                            dateOfBirth = child.DateOfBirth,
                            gender = child.Gender,
                            bloodType = child.BloodType,
                            medicalConditions = child.MedicalConditions,
                            allergies = child.Allergies,
                            notes = child.Notes
                        }
                    }
                };

                var response = await _graphqlClient.SendMutationAsync<dynamic>(graphQLRequest);

                if (response?.Errors != null && response.Errors.Length > 0)
                {
                    Console.WriteLine("GraphQL Errors:");
                    foreach (var error in response.Errors)
                    {
                        Console.WriteLine(error.Message);
                    }
                    return null;
                }

                if (response?.Data != null && response.Data.createChild != null)
                {
                    int result = response.Data.createChild;
                    Console.WriteLine($"Kết quả tạo child: {result}");

                    if (result > 0)
                    {
                        // Sau khi tạo thành công, lấy thông tin child vừa tạo
                        var createdChild = await GetChildById(child.ChildId);
                        if (createdChild != null)
                        {
                            Console.WriteLine($"Đã tạo thành công thông tin trẻ: {createdChild.FullName}");
                            return createdChild;
                        }
                    }
                }

                Console.WriteLine("Không thể tạo thông tin trẻ.");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi tạo thông tin trẻ: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                return null;
            }
        }

        public async Task<bool> UpdateChild(Child child)
        {
            if (child == null)
            {
                Console.WriteLine("Child object is null.");
                return false;
            }

            try
            {
                var graphQLRequest = new GraphQLRequest
                {
                    Query = @"
            mutation UpdateChild($childId: UUID!, $input: ChildInput!) {
                updateChild(childId: $childId, input: $input)
            }",
                    Variables = new
                    {
                        childId = child.ChildId.ToString(),
                        input = new
                        {
                            childId = child.ChildId.ToString(),
                            userId = child.UserId,
                            fullName = child.FullName,
                            dateOfBirth = child.DateOfBirth,
                            gender = child.Gender,
                            bloodType = child.BloodType,
                            medicalConditions = child.MedicalConditions,
                            allergies = child.Allergies,
                            notes = child.Notes
                        }
                    }
                };

                var response = await _graphqlClient.SendMutationAsync<dynamic>(graphQLRequest);

                if (response?.Errors != null && response.Errors.Length > 0)
                {
                    Console.WriteLine("GraphQL Errors:");
                    foreach (var error in response.Errors)
                    {
                        Console.WriteLine(error.Message);
                    }
                    return false;
                }

                if (response?.Data != null && response.Data.updateChild != null)
                {
                    int result = response.Data.updateChild;
                    Console.WriteLine($"Kết quả cập nhật: {result}");
                    return result > 0;
                }

                Console.WriteLine("Không có dữ liệu phản hồi từ GraphQL.");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi cập nhật thông tin trẻ: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                return false;
            }
        }


        public async Task<bool> DeleteChild(Guid childId)
        {
            try
            {
                var graphQLRequest = new GraphQLRequest
                {
                    Query = @"
                    mutation DeleteChild($childId: UUID!) {
                        deleteChild(childId: $childId)
                    }",
                    Variables = new
                    {
                        childId = childId.ToString()
                    }
                };

                var response = await _graphqlClient.SendMutationAsync<dynamic>(graphQLRequest);

                if (response != null && response.Data != null)
                {
                    bool success = response.Data.deleteChild;
                    Console.WriteLine($"Kết quả xóa: {(success ? "Thành công" : "Thất bại")}");
                    return success;
                }

                Console.WriteLine($"Không thể xóa thông tin trẻ với ID: {childId}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi xóa thông tin trẻ: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                return false;
            }
        }





    }
}