$("#fileUpload").on('change', function () {
	var files = $('#fileUpload').prop("files");
	var url = "/DataImportWizard";
	formData = new FormData();
	formData.append("file", files[0]);

	jQuery.ajax({
		type: 'POST',
		url: url,
		data: formData,
		cache: false,
		contentType: false,
		processData: false,
		success: function (repo) {
			if (repo.status == "success") {
				alert("File : " + repo.filename + " is uploaded successfully");
			}
		},
		error: function (error) {
			alert(JSON.stringify(error));
		}
	});
});