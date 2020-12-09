import React, { useState } from 'react';
import { Formik, Form, Field, ErrorMessage, useFormik } from 'formik';
import VideoCard from '../../VideoCard/VideoCard';

import './AddVideo.css';
import {
  Avatar,
  Box,
  Button,
  Container,
  CssBaseline,
  Grid,
  Snackbar,
  TextField,
  Typography,
} from '@material-ui/core';
import Alert from '@material-ui/lab/Alert';
import VideoCallTwoToneIcon from '@material-ui/icons/VideoCallTwoTone';

import * as yup from 'yup';
import { useStyles } from '../../Forms/Register/styles';
import { Redirect } from 'react-router-dom';
import Copyright from '../../Forms/Register/Copyright';

const AddVideo = () => {
  const [Title, setTitle] = useState('');
  const [Description, setDescription] = useState('');
  const [errorMessage, setErrorMessage] = useState('');
  const [Thumbnail, setThumbnail] = useState(null);
  const maxNumber = 1;
  const [Video, setVideo] = useState(null);
  const [open, setOpen] = useState(false);
  const [success, setSuccess] = useState(false);
  const classes = useStyles();
  const handleClose = (event, reason) => {
    if (reason === 'clickaway') {
      return;
    }

    setOpen(false);
  };
  const onChange = (Thumbnail, addUpdateIndex) => {
    // data for submit
    console.log(Thumbnail, addUpdateIndex);
    setThumbnail(Thumbnail);
  };
  const validationSchema = yup.object({
    Title: yup
      .string()
      .min(2, 'Title must contain at least 2 characters')
      .max(100, 'Title must contain max 100 characters')
      .required('Title is required'),
    Description: yup.string(),
  });
  const formik = useFormik({
    initialValues: {
      Title: Title || '',
      Description: Description || '',
      Thumbnail: Thumbnail || null,
      Video: Video || null,
    },
    validationSchema: validationSchema,
    onSubmit: async (values) => {
      await setTimeout(async () => {
        try {
          const currentUser = JSON.parse(
            window.localStorage.getItem('CurrentUser')
          );

          //TODO: implement creating video
          const requestOptions = {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
              video: [{}],
              thubmnail: [{}],
              userId: currentUser.id,
              userSecret: currentUser.secret,
              shown: true,
              title: values.Title,
              description: values.Description,
            }),
          };
          fetch(
            'https://youtube278.azurewebsites.net/api/Video',
            requestOptions
          );
          console.log(values);

          setErrorMessage('Video Uploaded Successfully');
          setSuccess(true);
          setOpen(true);

          //TODO: implement update video

          setErrorMessage('Video Uploaded Successfully');
          setSuccess(true);
          setOpen(true);
        } catch (error) {
          setErrorMessage(
            'Critical error occured, check your internet connection'
          );
          setSuccess(false);
          setOpen(true);
        }
      }, 100);
    },
  });

  return (
    <Container component="main" maxWidth="xs">
      <CssBaseline />
      <Snackbar open={open} autoHideDuration={6000} onClose={handleClose}>
        <Alert onClose={handleClose} severity={success ? 'success' : 'error'}>
          {errorMessage}
        </Alert>
      </Snackbar>
      <div className={classes.paper}>
        <Avatar className={classes.avatar}>
          <VideoCallTwoToneIcon />
        </Avatar>{' '}
        <Typography component="h1" variant="h5">
          Upload{' '}
        </Typography>{' '}
        <form className={classes.form} onSubmit={formik.handleSubmit}>
          <Grid container spacing={3}>
            <Grid item xs={12}>
              <TextField
                variant="outlined"
                required
                fullWidth
                id="title"
                label="Title"
                name="Title"
                value={formik.values.Title}
                onChange={formik.handleChange}
                error={formik.touched.Title && Boolean(formik.errors.Title)}
                helperText={formik.touched.Title && formik.errors.Title}
              />
            </Grid>
            <Grid item xs={12}>
              <TextField
                variant="outlined"
                required
                fullWidth
                rows={2}
                name="Description"
                label="Description"
                type="text"
                id="description"
                multiline
                value={formik.values.Description}
                onChange={formik.handleChange}
                error={
                  formik.touched.Description &&
                  Boolean(formik.errors.Description)
                }
                helperText={
                  formik.touched.Description && formik.errors.Description
                }
                inputProps={{ className: classes.textarea }}
              />
            </Grid>{' '}
            <Grid item xs={12}></Grid>
          </Grid>{' '}
          <Button
            type="submit"
            fullWidth
            variant="contained"
            color="primary"
            className={classes.submit}
          >
            Upload{' '}
          </Button>{' '}
        </form>{' '}
      </div>{' '}
      <Box mt={5}>
        <Copyright />
      </Box>{' '}
      {/* {success ? <Redirect to="../../Pages/RecommendedVideos" /> : ''} */}
    </Container>
  );
};

export default AddVideo;

// function AddVideo() {
//     return (
//         <div className="addVideo">
//             <h2>Upload your Video</h2>
//             <Formik>
//             <Field name="title"/>

//             </Formik>

//         </div>
//     )
// }

// export default AddVideo;
